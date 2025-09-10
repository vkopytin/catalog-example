using Db;
using Errors;
using Models;
using MongoDB.Driver.Linq;

namespace Services;

using static Consts.Consts;

public class ArticleBlocksService : IArticleBlocksService
{
  private readonly MongoDbContext dbContext;

  public ArticleBlocksService(MongoDbContext dbContext)
  {
    this.dbContext = dbContext;
  }

  public async Task<(ArticleBlockModel[]? articles, ServiceError? err)> ListArticleBlocks(int from = 0, int limit = 20)
  {
    var query
    = from b in dbContext.ArticleBlocks.AsEnumerable()
      join m in dbContext.ArticleBlocks on b.MediaId equals m.Id into media
      from sub in media.DefaultIfEmpty()
      orderby b.CreatedAt descending
      select b;
    var blocks = query.Skip(from).Take(limit).Select(a => a.ToModel()).ToArray();

    return (blocks, null);
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
