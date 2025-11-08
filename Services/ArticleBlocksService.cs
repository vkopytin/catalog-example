using Db;
using Errors;
using Microsoft.EntityFrameworkCore;
using Models;
using MongoDB.Driver.Linq;

namespace Services;

public class ArticleBlocksService : IArticleBlocksService
{
  private readonly MongoDbContext dbContext;

  public ArticleBlocksService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<(ArticleBlockModel[]? articles, ServiceError? err)> ListArticleBlocks(int from = 0, int limit = 20)
  {
    await Task.Delay(1);
    var query
    = from b in dbContext.ArticleBlocks.AsEnumerable()
      orderby b.CreatedAt descending
      select b;
    var blocks = query.Skip(from).Take(limit).Select(a => a.ToModel()).ToArray();

    return (blocks, null);
  }

  public async Task<(ArticleBlockModel? block, ServiceError? err)> GetArticleBlocById(int blockId)
  {
    var block = await dbContext.ArticleBlocks.FindAsync(blockId);

    return (block?.ToModel(), null);
  }

  public async Task<(ArticleBlockModel? block, ServiceError? err)> CreateArticleBlock(ArticleBlockModel block)
  {
    if (block is null)
    {
      return (null, new ServiceError("Can't create article block. Invalid data provided."));
    }

    var lastMediaId = await this.dbContext.ArticleBlocks.Select(b => b.Id).MaxAsync();
    var record = (block with { Id = lastMediaId + 1 }).ToRecord();
    record.Assign(block);
    record.CreatedAt = DateTime.UtcNow;

    var res = await dbContext.ArticleBlocks.AddAsync(record);
    await dbContext.SaveChangesAsync();

    record = await dbContext.ArticleBlocks.FindAsync(res.Entity.Id);

    if (record is null)
    {
      return (null, new ServiceError($"Create error. No block with the id ({res.Entity.Id}) was found"));
    }

    return (record.ToModel(), null);
  }

  public async Task<(ArticleBlockModel? block, ServiceError? err)> UpdateArticleBlock(int id, ArticleBlockModel block)
  {
    if (block is null)
    {
      return (null, new ServiceError("Can't update article block. Invalid data provided."));
    }
    var record = await dbContext.ArticleBlocks.FindAsync(id);
    if (record is null)
    {
      return (null, new ServiceError($"Update error. No block with the id ({id}) was found"));
    }

    record.Assign(block);
    record.UpdatedAt = DateTime.UtcNow;

    dbContext.ArticleBlocks.Update(record);

    await dbContext.SaveChangesAsync();

    record = await dbContext.ArticleBlocks.FindAsync(id);
    if (record is null)
    {
      return (null, new ServiceError($"Update error. No block with the id ({id}) was found"));
    }

    return (record.ToModel(), null);
  }
}
