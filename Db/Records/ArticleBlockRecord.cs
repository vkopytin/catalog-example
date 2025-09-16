using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Records;

[Table("ArticleBlock")]
public class ArticleBlockRecord : BaseEntity<int>
{
  public string? Title { get; set; }
  public string? Description { get; set; }

  public string? Origin { get; set; }

  public DateTime UpdatedAt { get; set; }

  #region Navigation Properties
  public Guid? ArticleId { get; set; }

  [ForeignKey("ArticleId")]
  public ArticleRecord? Article { get; set; }

  #endregion

  #region Parent Id - means this block is media
  public int? ParentId { get; set; }
  [ForeignKey("ParentId")]
  public ArticleBlockRecord? Media { get; set; }
  public ArticleBlockRecord? Block { get; set; }
  public int? Width { get; set; }
  public int? Height { get; set; }
  public string? SourceUrl { get; set; }
  public string? FileName { get; set; }
  #endregion

  public string? Rank { get; set; }
}