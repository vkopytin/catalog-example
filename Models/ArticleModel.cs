using Db.Records;

namespace Models;

public record ArticleModel
(
  Guid Id,
  string? Title,
  string? Description,
  DateTime CreatedAt,
  int MediaId,
  string? Origin,
  ArticleBlockModel? Media,
  ICollection<ArticleBlockModel> Blocks,
  WebSiteModel[] WebSites,
  WebSiteArticleRecord[] WebSiteArticles
);
