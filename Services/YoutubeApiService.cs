using System.Text.Json;
using Db;
using Db.Records;
using Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Models.YoutubeApi;

namespace Services.YoutubeApi;

public class YoutubeApiService
{
  private readonly MongoDbContext dbContext;
  private readonly HttpClient httpClient;
  private readonly AuthorizationTokensService authorizationTokensService;

  public YoutubeApiService(
    MongoDbContext dbContext,
    AuthorizationTokensService authorizationTokensService,
    HttpClient httpClient
  )
  {
    this.dbContext = dbContext;
    this.authorizationTokensService = authorizationTokensService;
    this.httpClient = httpClient;
  }

  public async Task<(YoutubeChannelResponse? result, ServiceError? error)> ListChannels(string sequrityGroupId)
  {
    var (token, tokenError) = await this.authorizationTokensService.GetAccessToken(sequrityGroupId);
    if (token is null)
    {
      return (null, tokenError ?? new ServiceError("Unknown error"));
    }

    var (subscriptions, err) = await this.ListYoutubeSubscriptions(token);
    if (subscriptions is null)
    {
      return (null, new ServiceError(err ?? "Unknown error"));
    }

    try
    {
      var channels = JsonSerializer.Deserialize<YoutubeChannelsResponse>(subscriptions, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      var channelsQuery = from c in dbContext.YoutubeChannels
                          where c.SecurityGroupId == sequrityGroupId
                          select c;

      var res = new List<YoutubeChannelRecord>();

      foreach (var channel in channels.Items)
      {
        var existingRecord = await channelsQuery.FirstOrDefaultAsync(c => c.Id == channel.Id);
        if (existingRecord is null)
        {
          existingRecord = new YoutubeChannelRecord
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
            SecurityGroupId = sequrityGroupId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsSubscribed = true
          };

          await dbContext.YoutubeChannels.AddAsync(existingRecord);
        }
        else
        {
          existingRecord.Title = channel.Snippet.Title;
          existingRecord.Description = channel.Snippet.Description;
          existingRecord.PublishedAt = channel.Snippet.PublishedAt;
          existingRecord.ThumbnailUrl = channel.Snippet.Thumbnails?.High?.Url
              ?? channel.Snippet.Thumbnails?.Medium?.Url
              ?? channel.Snippet.Thumbnails?.Default?.Url
              ?? string.Empty;
          existingRecord.UpdatedAt = DateTime.UtcNow;
          existingRecord.IsSubscribed = true;

          dbContext.YoutubeChannels.Update(existingRecord);
        }

        res.Add(existingRecord);
      }

      await dbContext.SaveChangesAsync();

      return (new(res), null);
    }
    catch (Exception ex)
    {
      return (null, new ServiceError($"Failed to deserialize YouTube channels response: {ex.Message}"));
    }
  }


  private async Task<(string? result, string? error)> ListYoutubeSubscriptions(string accessToken)
  {
    var parts = Uri.EscapeDataString("snippet");
    var subscriptionsUri = $"https://www.googleapis.com/youtube/v3/subscriptions?part={parts}&mine=true";
    var subscriptionsRequest = new HttpRequestMessage(HttpMethod.Get, subscriptionsUri);
    subscriptionsRequest.Headers.Add("Authorization", $"Bearer {accessToken}");

    var subscriptionsResponse = await httpClient.SendAsync(subscriptionsRequest);
    var subscriptionsResponseContent = await subscriptionsResponse.Content.ReadAsStringAsync();

    if (subscriptionsResponse.IsSuccessStatusCode is false)
    {
      return (null, $"Failed to get YouTube subscriptions: {subscriptionsResponseContent}");
    }

    return (subscriptionsResponseContent, null);
  }
}
