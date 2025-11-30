using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text.Json;
using Dapper;
using Db;
using Db.Records;
using Errors;
using Models;
using Services.ImgBB;

namespace Services;

public class MediaLibraryService : IMediaLibraryService
{
  private readonly MainSettings settings;
  private readonly MongoDbContext dbContext;
  private readonly IDbConnectionFactory dbConnectionFactory;
  private readonly ImgBBConfig imgBB;

  public MediaLibraryService(
    MainSettings settings,
    MongoDbContext dbContext,
    IDbConnectionFactory dbConnectionFactory,
    ImgBBConfig imgBB
  )
  {
    this.settings = settings;
    this.dbContext = dbContext;
    this.dbConnectionFactory = dbConnectionFactory;
    this.imgBB = imgBB;
  }

  public async Task<(ArticleBlockModel?, ServiceError?)> Create(int blockId, Stream stream, string contentType, string fileName)
  {
    if (contentType.StartsWith("image/"))
    {
      return await this.UploadImage(blockId, stream, contentType, fileName);
    }
    if (contentType.StartsWith("video/"))
    {
      return (null, new ServiceError("Video upload not implemented"));
    }
    if (contentType.StartsWith("application/pdf"))
    {
      return await this.UploadPdf(blockId, stream, contentType, fileName);
    }
    return (null, new ServiceError($"Unsupported media type: {contentType}"));
  }

  public async Task<(FileData? result, ServiceError? error)> GetDocument(Guid documentId)
  {
    using var connection = await this.dbConnectionFactory.CreateConnectionAsync();
    var document = await connection.QuerySingleOrDefaultAsync<FileData>("""
      SELECT * FROM documents
      WHERE id = @Id
      """,
      new
      {
        Id = documentId
      }
    );

    if (document is null)
    {
      return (null, new ServiceError("Document not found"));
    }

    return (document, null);
  }

  private async Task<(ArticleBlockModel?, ServiceError?)> UploadPdf(int blockId, Stream stream, string contentType, string fileName)
  {
    var existing = this.dbContext.ArticleBlocks.Find(blockId);
    if (existing is null)
    {
      return (null, new ServiceError("Can't find block to update"));
    }

    try
    {
      using var connection = await this.dbConnectionFactory.CreateConnectionAsync();
      using var file = new BinaryReader(stream);
      var bytes = file.ReadBytes((int)stream.Length);
      var fileNameWithGuid = $"{Guid.NewGuid()}-{fileName}";
      await connection.ExecuteAsync(
        """
        INSERT INTO documents (id, FileName, ContentType, Data, CreatedAt)
        VALUES (@Id, @FileName, @ContentType, @Data, @CreatedAt)
        """,
        new
        {
          Id = Guid.NewGuid(),
          FileName = fileNameWithGuid,
          ContentType = contentType,
          Data = bytes,
          CreatedAt = DateTime.UtcNow
        }
      );

      var document = await connection.QuerySingleAsync<FileData>(
        """
        SELECT * FROM documents
        WHERE FileName = @FileName
        """,
        new { FileName = fileNameWithGuid }
      );

      existing.SourceUrl = null;
      existing.Origin = $"{this.settings.Media.BaseUrl}/media/documents/{document.Id}";
      this.dbContext.ArticleBlocks.Update(existing);
      await this.dbContext.SaveChangesAsync();

      return (existing.ToModel(), null);
    }
    catch (Exception ex)
    {
      return (null, new ServiceError($"PDF upload error: {ex.Message}"));
    }
  }

  private async Task<(ArticleBlockModel?, ServiceError?)> UploadImage(int blockId, Stream stream, string contentType, string fileName)
  {
    var existing = this.dbContext.ArticleBlocks.Find(blockId);
    if (existing is null)
    {
      return (null, new ServiceError("Can't find block to update"));
    }
    var (imgBB, error) = await this.CreateImageBB(stream, contentType, fileName);
    if (imgBB is null)
    {
      return (null, new ServiceError(error ?? "Unknown error"));
    }

    if (imgBB.Data is null)
    {
      return (null, new ServiceError("Strange error"));
    }

    existing.SourceUrl = null;
    existing.Origin = imgBB.Data.Url;
    existing.Width = imgBB.Data.Width;
    existing.Height = imgBB.Data.Height;

    this.dbContext.ArticleBlocks.Update(existing);

    await this.dbContext.SaveChangesAsync();

    return (existing.ToModel(), null);
  }

  private async Task<(ImgBBUploadResult?, string? error)> CreateImageBB(Stream stream, string contentType, string fileName)
  {
    using var httpClient = new HttpClient();
    using MultipartFormDataContent multipartFormData = [];
    using var base64Stream = new CryptoStream(stream, new ToBase64Transform(), CryptoStreamMode.Read);

    httpClient.BaseAddress = new Uri(this.imgBB.BaseAddress);
    multipartFormData.Add(new StreamContent(base64Stream), "image");
    multipartFormData.Add(new StringContent(imgBB.Key), "key");
    multipartFormData.Add(new StringContent("description"), contentType);
    HttpResponseMessage httpResult = await httpClient.PostAsync($"/1/upload?name=image", multipartFormData);

    if (httpResult.IsSuccessStatusCode)
    {
      var retValue = await httpResult.Content.ReadAsStringAsync();
      var reyValue = JsonSerializer.Deserialize<ImgBBUploadResult>(retValue);

      return (reyValue, null);
    }

    var res = await httpResult.Content.ReadAsStringAsync();

    return (null, res);
  }
}
