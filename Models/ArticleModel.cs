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
  ICollection<WebSiteModel> WebSites
);

public static class ArticleModelExtensions
{
  public static ArticleModel ToArticle(this CreateArticleRequest model)
  {
    return new(
      Id: model.Id,
      Title: model.Title,
      Description: model.Description,
      CreatedAt: model.CreatedAt,
      MediaId: model.MediaId,
      Origin: model.Origin,
      Media: model.Media,
      Blocks: model.Blocks,
      []
    );
  }
}
