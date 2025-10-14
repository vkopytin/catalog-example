using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Records;

[Table("Article")]
public class ArticleRecord : BaseEntity<Guid>
{
  public string? Title { get; set; }
  public string? Description { get; set; }

  public string? Origin { get; set; }

  public DateTime UpdatedAt { get; set; }
  public DateTime? RemovedAt { get; set; }

  public int? CategoryId { get; set; }
  [ForeignKey("CategoryId")]
  public CategoryRecord? Category { get; set; }

  public int MediaId { get; set; }
  [ForeignKey("MediaId")]
  public ArticleBlockRecord? Media { get; set; }

  [InverseProperty("Article")]
  public ICollection<ArticleBlockRecord> Blocks { get; } = [];

  public ICollection<WebSiteRecord> WebSites { get; set; } = [];
  public ICollection<WebSiteArticleRecord> WebSiteArticles { get; set; } = [];
}
