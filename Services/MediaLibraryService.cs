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

  public MediaLibraryService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<(ArticleBlockModel?, ServiceError?)> Create(Stream stream, string contentType, string fileName)
  {
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

    var media = items.Where(a => a.SourceUrl == imgBB.Data.Url).Select(b => b.ToModel()).FirstOrDefault();

    if (media is not null)
    {
      return (media, null);
    }

    var lastRecord = this.dbContext.ArticleBlocks.Select(b => b.Id).Max();

    var record = new ArticleBlockRecord
    {
      Id = lastRecord + 1,
      SourceUrl = imgBB.Data.Url
    };
    var res = await this.dbContext.ArticleBlocks.AddAsync(record);
    await this.dbContext.SaveChangesAsync();

    record = this.dbContext.ArticleBlocks.Find(res.Entity.Id);
    if (record is null)
    {
      return (null, new ServiceError($"Create error. No article block with the id ({res.Entity.Id}) was found"));
    }

    return (record.ToModel(), null);
  }

  private async Task<(ImgBBUploadResult?, string? error)> CreateImageBB(Stream stream, string title, string fileName)
  {
    using var httpClient = new HttpClient();
    using MultipartFormDataContent multipartFormData = [];
    using var base64Stream = new CryptoStream(stream, new ToBase64Transform(), CryptoStreamMode.Read);

    httpClient.BaseAddress = new Uri("https://api.imgbb.com");
    multipartFormData.Add(new StreamContent(base64Stream), "image");
    multipartFormData.Add(new StringContent("74e2ce10556318a587dadc06e5171c94"), "key");
    multipartFormData.Add(new StringContent("description"), title);
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
