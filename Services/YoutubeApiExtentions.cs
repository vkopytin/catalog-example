using Db.Records;
using Models.YoutubeApi;

namespace Services.YoutubeApi;

public static class YoutubeApiExtentions
{
  public static YoutubeChannelRecord ToRecord(this YoutubeSubscription channel, string securityGroupId)
  {
    return new YoutubeChannelRecord
    {
      Id = channel.Id,
      ChannelId = channel.Snippet.ResourceId.ChannelId,
      Title = channel.Snippet.Title,
      Description = channel.Snippet.Description,
      PublishedAt = channel.Snippet.PublishedAt,
      ThumbnailUrl = channel.Snippet.Thumbnails?.High?.Url
              ?? channel.Snippet.Thumbnails?.Medium?.Url
              ?? channel.Snippet.Thumbnails?.Default?.Url
              ?? string.Empty,
      SecurityGroupId = securityGroupId,
      CreatedAt = DateTime.UtcNow,
      UpdatedAt = DateTime.UtcNow,
      IsSubscribed = true
    };
  }
}