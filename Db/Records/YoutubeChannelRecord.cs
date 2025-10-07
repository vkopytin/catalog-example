namespace Db.Records;

public class YoutubeChannelRecord : BaseEntity<string>
{
  public string ChannelId { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public DateTime PublishedAt { get; set; }
  public string ThumbnailUrl { get; set; }
  public DateTime UpdatedAt { get; set; }

  public string SecurityGroupId { get; set; }
}
