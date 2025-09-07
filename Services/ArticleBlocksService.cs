using Db;
using Errors;
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

    public Task<(ArticleBlockModel[]? articles, ServiceError? err)> ListArticleBlocks(int from = 0, int limit = 20)
    {
        var query
        = from b in dbContext.ArticleBlocks.AsEnumerable()
          join m in dbContext.ArticleBlocks on b.MediaId equals m.Id into media
          from sub in media.DefaultIfEmpty()
          orderby b.CreatedAt descending
          select b;
        var blocks = query.Skip(from).Take(limit).ToArray();

        return Task.FromResult<(ArticleBlockModel[]?, ServiceError?)>(
          (blocks.Select(a => a.ToModel()).ToArray(), null)
        );
    }
}