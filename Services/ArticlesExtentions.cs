using Db.Records;
using Models;

namespace Services;

public static class ArticlesExtentions
{
  public static ArticleModel ToModel(this ArticleRecord record)
  {
    return new(
      Id: record.Id,
      Title: record.Title,
      Description: record.Description,
      CreatedAt: record.CreatedAt,
      MediaId: record.MediaId,
      Origin: record.Origin,
      Media: record.Media?.ToModel(),
      Blocks: [.. record.Blocks.Select(b => b.ToModel())],
      WebSites: [.. record.WebSites.Select(w => w.ToModel())],
      WebSiteArticles: []
    );
  }

  public static ArticleRecord ToRecord(this ArticleModel model)
  {
    return new()
    {
      Id = model.Id,
      Title = model.Title,
      Description = model.Description,
      Origin = model.Origin,
      MediaId = model.MediaId
    };
  }

  public static void Assign(this ArticleRecord record, ArticleModel model)
  {
    record.Title = model.Title;
    record.Description = model.Description;
    record.Origin = model.Origin;
    record.MediaId = model.MediaId;
  }
}
