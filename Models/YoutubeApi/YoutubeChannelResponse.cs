using System.Text.Json.Serialization;
using Db.Records;

namespace Models.YoutubeApi;

public record YoutubeChannelResponse(
  [property: JsonPropertyName("items")]
  List<YoutubeChannelRecord> Items
);