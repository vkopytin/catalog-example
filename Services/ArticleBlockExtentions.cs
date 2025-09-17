using Db.Records;
using Models;

namespace Services;

public static class ArticleBlockExtentions
{
  public static ArticleBlockModel ToModel(this ArticleBlockRecord record)
  {
    return new(
      Id: record.Id,
      Title: record.Title,
      Description: record.Description,
      Origin: record.Origin,
      SourceUrl: record.SourceUrl,
      FileName: record.FileName,
      MediaId: record.MediaId,
      Width: record.Width,
      Height: record.Height,
      Media: record.Media?.ToModel()
    );
  }

  public static void Assign(this ArticleBlockRecord record, ArticleBlockModel model)
  {
    record.Title = model.Title;
    record.Description = model.Description;
    record.Origin = model.Origin;
  }
}
