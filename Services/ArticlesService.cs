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

  public async Task<(ArticleModel[]? articles, ServiceError? err)> ListOwnArticles(string securityGroupId, int skip = 0, int limit = 20)
  {
    var user = await this.dbContext.Users.FirstOrDefaultAsync(u => u.SecurityGroupId == MongoDB.Bson.ObjectId.Parse(securityGroupId));
    if (user is null)
    {
      return ([], null);
    }

    var webSiteIdsQuery = from a in this.dbContext.WebSites.AsEnumerable()
                          join parent in dbContext.WebSites on a.ParentId equals parent.Id into parents
                          where a.UserId == user.Id
                          select a;
    var webSites = webSiteIdsQuery.ToArray();
    var webSiteIds = new List<Guid>();
    foreach (var site in webSites)
    {
      var webSite = site;
      while (webSite is not null)
      {
        webSiteIds.Add(webSite.Id);
        if (webSite.ParentId is null)
        {
          break;
        }
        webSite = webSites.FirstOrDefault(s => s.Id == webSite.ParentId);
      }
    }

    var query
    = from a in dbContext.Articles.AsEnumerable()
      join m in dbContext.ArticleBlocks on a.MediaId equals m.Id into mleft
      from sub in mleft.DefaultIfEmpty()
      where webSiteIds.Contains(
        (from wsa in dbContext.WebSiteArticles.AsEnumerable()
         where wsa.ArticleId == a.Id
         select wsa.WebSiteId).FirstOrDefault()
      )
      orderby a.CreatedAt descending
      select a;
    var articles = query.Skip(skip).Take(limit).Select(a => a.ToModel()).ToArray();

    return (articles, null);
  }

  public Task<(ArticleModel[]? articles, ServiceError? err)> ListArticlesBySiteId(Guid siteId, int from = 0, int limit = 20)
  {
    var query
    = from a in dbContext.Articles.AsEnumerable()
      join m in dbContext.ArticleBlocks on a.MediaId equals m.Id into mleft
      from sub in mleft.DefaultIfEmpty()
      join wsa in dbContext.WebSiteArticles on a.Id equals wsa.ArticleId
      where wsa.WebSiteId == siteId
      orderby a.CreatedAt descending
      select a;
    var articles = query.Skip(from).Take(limit).Select(a => a.ToModel()).ToArray();

    return Task.FromResult<(ArticleModel[]? articles, ServiceError? err)>((articles, null));
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
    else if (article.Media is not null)
    {
      var mediaRecord = article.Media.ToRecord();
      this.dbContext.ArticleBlocks.Update(mediaRecord);
      await this.dbContext.SaveChangesAsync();
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
      join s in dbContext.WebSiteArticles on a.Id equals s.ArticleId into wsleft
      from wssub in wsleft.DefaultIfEmpty()
      join w in dbContext.WebSites on wssub?.WebSiteId equals w.Id into wleft
      from wsub in wleft.DefaultIfEmpty()
      where a.Id == id
      select a;

    return query.FirstOrDefault();
  }
}
