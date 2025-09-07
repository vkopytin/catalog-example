using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Records;

[Table("WebSiteArticle")]
public class WebSiteArticleRecord : BaseEntity<int>
{
  public Guid ArticleId { get; set; }

  public Guid WebSiteId { get; set; }

  public virtual WebSiteRecord WebSite { get; set; }
  public virtual ArticleRecord Article { get; set; }

}
