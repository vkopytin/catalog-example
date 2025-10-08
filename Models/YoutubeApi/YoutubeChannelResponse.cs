using System.Text.Json.Serialization;
using Db.Records;

namespace Models.YoutubeApi;

public record YoutubeChannelResponse(
  [property: JsonPropertyName("items")]
  List<YoutubeChannelRecord> Items,
  [property: JsonPropertyName("limit")]
  int Limit,
  [property: JsonPropertyName("total")]
  int Total
);