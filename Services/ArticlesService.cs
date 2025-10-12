using Db;
using Db.Records;
using Errors;
using Microsoft.EntityFrameworkCore;
using Models;
using MongoDB.Driver.Linq;

namespace Services;

using static Consts.Consts;

public class ArticlesService : IArticlesService
{
  private readonly MongoDbContext dbContext;

  public ArticlesService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<(ArticleModel[]? articles, ServiceError? err)> ListArticles(int skip = 0, int limit = 20)
  {
    await Task.Delay(1);

    var query
    = from a in dbContext.Articles.AsEnumerable()
      join m in dbContext.ArticleBlocks on a.MediaId equals m.Id into mleft
      from sub in mleft.DefaultIfEmpty()
      orderby a.CreatedAt descending
      select a.ToModel();
    var articles = query.Skip(skip).Take(limit).ToArray();

    return (articles, null);
  }

  public async Task<(ArticleModel? article, ServiceError? err)> GetArticle(Guid id)
  {
    await Task.Delay(1);

    try
    {
      var article = this.GetArticleInternal(id);

      return (article?.ToModel(), null);
    }
    catch (Exception ex)
    {
      return (null, new ServiceError($"Get error. {ex.Message}"));
    }
  }

  public async Task<(ArticleModel? article, ServiceError? err)> CreateArticle(ArticleModel article)
  {
    if (article is null)
    {
      return (null, new ServiceError("Can't create article. Invalid data provided."));
    }

    var record = (article with { Id = Guid.NewGuid() }).ToRecord();
    record.CreatedAt = DateTime.UtcNow;
    record.UpdatedAt = DateTime.UtcNow;

    var res = await this.dbContext.Articles.AddAsync(record);
    await this.dbContext.SaveChangesAsync();

    record = await dbContext.Articles.FindAsync(res.Entity.Id);
    if (record is null)
    {
      return (null, new ServiceError($"Create error. No item with the id ({res.Entity.Id}) was found"));
    }

    return (record.ToModel(), null);
  }

  public async Task<(ArticleModel? article, ServiceError? err)> UpdateArticle(Guid id, ArticleModel article)
  {
    if (article == null)
    {
      return (null, new ServiceError("Invalid article"));
    }

    var record = await dbContext.Articles.FindAsync(article.Id);
    if (record is null)
    {
      return (null, new ServiceError($"Can't update. No item with the id ({article.Id}) was found"));
    }

    record.Assign(article);
    if (article.MediaId == 0 && article.Media != null)
    {
      var media = article.Media;
      if (media.Id == 0)
      {
        var mediaRecord = media.ToRecord();
        var lastMediaId = await this.dbContext.ArticleBlocks.Select(b => b.Id).MaxAsync();
        mediaRecord.Id = lastMediaId + 1;
        var res = await this.dbContext.ArticleBlocks.AddAsync(mediaRecord);
        await this.dbContext.SaveChangesAsync();
        record.MediaId = res.Entity.Id;
      }
      else
      {
        record.MediaId = media?.Id ?? 0;
      }
    }

    record.UpdatedAt = DateTime.UtcNow;

    this.dbContext.Articles.Update(record);

    try
    {
      await this.dbContext.SaveChangesAsync();
      record = this.GetArticleInternal(article.Id);
      if (record is null)
      {
        return (null, new ServiceError($"Update error. No item with the id ({article.Id}) was found"));
      }

      return (record.ToModel(), null);

    }
    catch (Exception ex)
    {
      return (null, new ServiceError($"Update error. {ex.Message}"));
    }
  }

  private ArticleRecord? GetArticleInternal(Guid id)
  {
    var query =
      from a in dbContext.Articles.AsEnumerable()
      join m in dbContext.ArticleBlocks on a.MediaId equals m.Id into mleft
      from sub in mleft.DefaultIfEmpty()
      where a.Id == id
      select a;

    return query.FirstOrDefault();
  }
}
