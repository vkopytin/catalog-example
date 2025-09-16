using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text.Json;
using Db;
using Db.Records;
using Errors;
using Models;
using Services.ImgBB;

namespace Services;

public class MediaLibraryService : IMediaLibraryService
{
  private readonly MongoDbContext dbContext;
  private readonly ImgBBConfig imgBB;

  public MediaLibraryService(MongoDbContext dbContext, ImgBBConfig imgBB)
  {
    this.dbContext = dbContext;
    this.imgBB = imgBB;
  }

  public async Task<(ArticleBlockModel?, ServiceError?)> Create(int blockId, Stream stream, string contentType, string fileName)
  {
    var existing = this.dbContext.ArticleBlocks.Find(blockId);
    if (existing is null)
    {
      return (null, new ServiceError("Can't find block to update"));
    }
    var (imgBB, error) = await this.CreateImageBB(stream, contentType, fileName);
    if (imgBB is null)
    {
      return (null, new ServiceError(error));
    }

    if (imgBB is null || imgBB.Data is null)
    {
      return (null, new ServiceError("Starange error"));
    }

    var items =
     from block in this.dbContext.ArticleBlocks.AsEnumerable()
     select block;

    var media = items.Where(a => a.ParentId == existing.Id).FirstOrDefault();

    var record = media;
    if (record is null)
    {
      var lastRecord = this.dbContext.ArticleBlocks.Select(b => b.Id).Max();

      record = new ArticleBlockRecord
      {
        Id = lastRecord + 1,
        ParentId = existing.Id,
        SourceUrl = imgBB.Data.Url,
        Width = imgBB.Data.Width,
        Height = imgBB.Data.Height,
      };
      var res = await this.dbContext.ArticleBlocks.AddAsync(record);
      record = res.Entity;
    }
    else
    {
      record.SourceUrl = imgBB.Data.Url;
      record.Width = imgBB.Data.Width;
      record.Height = imgBB.Data.Height;

      this.dbContext.ArticleBlocks.Update(record);
    }
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
