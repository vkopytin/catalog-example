namespace Models;

public record ArticleBlockModel
(
  int Id,
  string? Title,
  string? Description,
  string? Origin,
  string? SourceUrl,
  string? FileName,
  int? MediaId,
  int? Width,
  int? Height,
  ArticleBlockModel? Media
//ICollection<ArticleBlockModel> Blocks
);
