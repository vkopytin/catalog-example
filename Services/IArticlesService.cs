using Errors;
using Models;

namespace Services;

public interface IArticlesService
{
  Task<(ArticleModel[]? articles, ServiceError? err)> ListArticlesBySiteId(Guid siteId, int from = 0, int limit = 20);
  Task<(ArticleModel[]? articles, ServiceError? err)> ListOwnArticles(string securityGroupId, int from = 0, int limit = 20);
  Task<(ArticleModel? article, ServiceError? err)> GetArticle(Guid id);
  Task<(ArticleModel? article, ServiceError? err)> CreateArticle(ArticleModel article);
  Task<(ArticleModel? article, ServiceError? err)> UpdateArticle(Guid id, ArticleModel article);
}
