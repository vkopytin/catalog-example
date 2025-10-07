using System.Text.Json.Serialization;

namespace Models.YoutubeApi;

public record YoutubeThumbnail(
  [property: JsonPropertyName("url")]
  string Url
);

public record YoutubeThumbnailDetails(
  [property: JsonPropertyName("default")]
  YoutubeThumbnail? Default,
  [property: JsonPropertyName("medium")]
  YoutubeThumbnail? Medium,
  [property: JsonPropertyName("high")]
  YoutubeThumbnail? High
);

public record YoutubeResourceId(
  [property: JsonPropertyName("kind")]
  string Kind,
  [property: JsonPropertyName("channelId")]
  string ChannelId
);

public record YoutubeSubscriptionSnippet(
  [property: JsonPropertyName("channelId")]
  string ChannelId,
  [property: JsonPropertyName("title")]
  string Title,
  [property: JsonPropertyName("description")]
  string Description,
  [property: JsonPropertyName("publishedAt")]
  DateTime PublishedAt,
  [property: JsonPropertyName("resourceId")]
  YoutubeResourceId ResourceId,
  [property: JsonPropertyName("thumbnails")]
  YoutubeThumbnailDetails Thumbnails
);

public record YoutubeSubscription(
  [property: JsonPropertyName("id")]
  string Id,
  [property: JsonPropertyName("snippet")]
  YoutubeSubscriptionSnippet Snippet
);

public record YoutubePageInfo(
  [property: JsonPropertyName("totalResults")]
  int TotalResults,
  [property: JsonPropertyName("resultsPerPage")]
  int ResultsPerPage
);

public record YoutubeChannelsResponse(
  [property: JsonPropertyName("items")]
  List<YoutubeSubscription> Items,
  [property: JsonPropertyName("pageInfo")]
  YoutubePageInfo PageInfo,
  [property: JsonPropertyName("nextPageToken")]
  string? NextPageToken
);
