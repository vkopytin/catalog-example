using Errors;
using Models;

namespace Services;

public interface IArticleBlocksService
{
  Task<(ArticleBlockModel[]? articles, ServiceError? err)> ListArticleBlocks(int from = 0, int limit = 20);
  Task<(ArticleBlockModel? block, ServiceError? err)> UpdateArticleBlock(int id, ArticleBlockModel block);
}
