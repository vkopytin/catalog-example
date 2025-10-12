using Db.Records;
using Models;

namespace Services;

public static class ArticleBlockExtentions
{
  public static ArticleBlockRecord ToRecord(this ArticleBlockModel model)
  {
    return new()
    {
      Id = model.Id,
      Title = model.Title,
      Description = model.Description,
      Origin = model.Origin,
      SourceUrl = model.SourceUrl,
      FileName = model.FileName,
      MediaId = model.MediaId,
      MediaError = model.MediaError,
      Width = model.Width,
      Height = model.Height
    };
  }

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
      MediaError: record.MediaError,
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
    record.SourceUrl = model.SourceUrl;
    record.FileName = model.FileName;
    record.MediaId = model.MediaId;
    record.MediaError = model.MediaError;
    record.Width = model.Width;
    record.Height = model.Height;
  }
}
