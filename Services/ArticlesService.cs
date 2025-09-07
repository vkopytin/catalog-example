using Db;
using Errors;
using Models;
using MongoDB.Driver.Linq;

namespace Services;

public class ArticlesService : IArticlesService
{
  private readonly MongoDbContext dbContext;

  public ArticlesService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public Task<(ArticleModel[]? articles, ServiceError? err)> ListArticles(int from = 0, int limit = 20)
  {
    var query
    = from a in dbContext.Articles.AsEnumerable()
      join m in dbContext.ArticleBlocks on a.MediaId equals m.Id into mleft
      from sub in mleft.DefaultIfEmpty()
      orderby a.CreatedAt descending
      select a;
    var articles = query.Skip(from).Take(limit).ToArray();

    return Task.FromResult<(ArticleModel[]?, ServiceError?)>(
      (articles.Select(a => a.ToModel()).ToArray(), null)
    );
  }

  public async Task<(ArticleModel? article, ServiceError? err)> GetArticle(Guid id)
  {
    var article = await dbContext.Articles.FindAsync(id);

    return (article?.ToModel(), null);
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
    record.UpdatedAt = DateTime.UtcNow;

    this.dbContext.Articles.Update(record);

    await this.dbContext.SaveChangesAsync();

    record = await dbContext.Articles.FindAsync(article.Id);
    if (record is null)
    {
      return (null, new ServiceError($"Update error. No item with the id ({article.Id}) was found"));
    }

    return (record.ToModel(), null);
  }
}
