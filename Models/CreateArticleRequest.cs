using Db.Records;

namespace Models;

public record CreateArticleRequest
(
  Guid Id,
  string? Title,
  string? Description,
  DateTime CreatedAt,
  int MediaId,
  string? Origin,
  ArticleBlockModel? Media,
  ICollection<ArticleBlockModel> Blocks
);
