using System.Text.Json;
using Db;
using Db.Records;
using Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Models.YoutubeApi;
using System.Linq;

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

  public async Task<(YoutubeChannelRecord? result, ServiceError? error)> UnsubscribeChannel(string securityGroupId, string resourceId)
  {
    var (token, tokenError) = await this.authorizationTokensService.GetAccessToken(securityGroupId);
    if (token is null)
    {
      return (null, tokenError ?? new ServiceError("Unknown error"));
    }

    var (res, err) = await this.UnsubscribeYoutubeChannel(token, resourceId);

    if (res is null)
    {
      return (null, new ServiceError(err ?? "Unknown error"));
    }

    var record = await dbContext.YoutubeChannels.FirstOrDefaultAsync(c => c.Id == resourceId && c.SecurityGroupId == securityGroupId);
    if (record is not null)
    {
      record.IsSubscribed = false;
      record.UpdatedAt = DateTime.UtcNow;
      dbContext.YoutubeChannels.Update(record);

      await dbContext.SaveChangesAsync();

      return (record, null);
    }
    else
    {
      var (json, jsonError) = await this.GetYoutubeSubscriptionDetails(token, resourceId);

      if (json is null)
      {
        return (new YoutubeChannelRecord { Id = resourceId, IsSubscribed = false }, null);
      }

      var subscription = JsonSerializer.Deserialize<YoutubeSubscription>(json, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      if (subscription is null)
      {
        return (new YoutubeChannelRecord { Id = resourceId, IsSubscribed = false }, null);
      }

      record = new YoutubeChannelRecord
      {
        Id = subscription.Id,
        ChannelId = subscription.Snippet.ResourceId.ChannelId,
        Title = subscription.Snippet.Title,
        Description = subscription.Snippet.Description,
        PublishedAt = subscription.Snippet.PublishedAt,
        ThumbnailUrl = subscription.Snippet.Thumbnails?.High?.Url
          ?? subscription.Snippet.Thumbnails?.Medium?.Url
          ?? subscription.Snippet.Thumbnails?.Default?.Url
          ?? string.Empty,
        SecurityGroupId = securityGroupId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        IsSubscribed = false
      };
      await dbContext.YoutubeChannels.AddAsync(record);
      await dbContext.SaveChangesAsync();

      return (record, null);
    }
  }

  public async Task<(YoutubeChannelRecord? result, ServiceError? error)> SubscribeChannel(string securityGroupId, string channelId)
  {
    var (token, tokenError) = await this.authorizationTokensService.GetAccessToken(securityGroupId);
    if (token is null)
    {
      return (null, tokenError ?? new ServiceError("Unknown error"));
    }

    var (json, err) = await this.SubscribeYoutubeChannel(token, channelId);

    if (json is null)
    {
      return (null, new ServiceError(err ?? "Unknown error"));
    }

    try
    {
      var subscription = JsonSerializer.Deserialize<YoutubeSubscription>(json, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      if (subscription is null)
      {
        return (null, new ServiceError("Failed to deserialize YouTube channels response. Result is null."));
      }

      var existingRecord = await dbContext.YoutubeChannels.FirstOrDefaultAsync(c => c.Id == subscription.Id);
      if (existingRecord is not null)
      {
        existingRecord.IsSubscribed = true;
        existingRecord.UpdatedAt = DateTime.UtcNow;
        dbContext.YoutubeChannels.Update(existingRecord);

        await dbContext.SaveChangesAsync();
      }
      else
      {
        existingRecord = subscription.ToRecord(securityGroupId);

        await dbContext.YoutubeChannels.AddAsync(existingRecord);
        await dbContext.SaveChangesAsync();
      }

      return (existingRecord, null);
    }
    catch (Exception ex)
    {
      return (null, new ServiceError($"Failed to deserialize unsubscribed YouTube channels response: {ex.Message}"));
    }
  }

  public async Task<(YoutubeChannelResponse? result, ServiceError? error)> ListChannels(string securityGroupId, int from, int limit)
  {
    var (token, tokenError) = await this.authorizationTokensService.GetAccessToken(securityGroupId);
    if (token is null)
    {
      return (null, tokenError ?? new ServiceError("Unknown error"));
    }

    // js - btoa([8,12,16,0].map(a => String.fromCharCode(a)).join(''))
    var nextPage = Convert.ToBase64String([8, (byte)from, 16, 0]);

    var (json, err) = await this.ListYoutubeSubscriptions(token, nextPage, limit);
    if (json is null)
    {
      return (null, new ServiceError(err ?? "Unknown error"));
    }

    try
    {
      var subscriptions = JsonSerializer.Deserialize<YoutubeChannelsResponse>(json, new JsonSerializerOptions
      {
        PropertyNameCaseInsensitive = true
      });

      if (subscriptions is null)
      {
        return (null, new ServiceError("Failed to deserialize YouTube channels response. Result is null."));
      }

      var subscriptionsQuery =
        from c in dbContext.YoutubeChannels
        where c.SecurityGroupId == securityGroupId
        orderby c.Title
        select c;

      var existingChannels = await subscriptionsQuery.Skip(from).Take(limit).ToArrayAsync();
      foreach (var c in existingChannels)
      {
        c.IsSubscribed = false;
      }
      var res = new List<YoutubeChannelRecord>();

      foreach (var subscription in subscriptions.Items)
      {
        var existingRecord = await subscriptionsQuery.FirstOrDefaultAsync(c => c.Id == subscription.Id);
        if (existingRecord is null)
        {
          existingRecord = subscription.ToRecord(securityGroupId);

          await dbContext.YoutubeChannels.AddAsync(existingRecord);
        }
        else
        {
          existingRecord.Title = subscription.Snippet.Title;
          existingRecord.Description = subscription.Snippet.Description;
          existingRecord.PublishedAt = subscription.Snippet.PublishedAt;
          existingRecord.ThumbnailUrl = subscription.Snippet.Thumbnails?.High?.Url
              ?? subscription.Snippet.Thumbnails?.Medium?.Url
              ?? subscription.Snippet.Thumbnails?.Default?.Url
              ?? string.Empty;
          existingRecord.UpdatedAt = DateTime.UtcNow;
          existingRecord.IsSubscribed = true;

          dbContext.YoutubeChannels.Update(existingRecord);
        }

        res.Add(existingRecord);
      }

      await dbContext.SaveChangesAsync();

      var result = MergeItems(existingChannels, [.. res], 100);

      return (new(
        Items: [.. result],
        Limit: subscriptions.PageInfo.ResultsPerPage,
        Total: subscriptions.PageInfo.TotalResults
      ), null);
    }
    catch (Exception ex)
    {
      return (null, new ServiceError($"Failed to deserialize YouTube channels response: {ex.Message}"));
    }
  }

  private static YoutubeChannelRecord[] MergeItems(YoutubeChannelRecord[] left, YoutubeChannelRecord[] right, int limit)
  {
    var dict = left.ToDictionary(l => l.Id, l => l);

    foreach (var r in right)
    {
      dict[r.Id] = r;
    }

    return [.. dict.Values.OrderBy(c => c.Title).Take(limit)];
  }

  private async Task<(string? result, string? error)> GetYoutubeSubscriptionDetails(string accessToken, string channelId)
  {
    var parts = Uri.EscapeDataString("snippet,contentDetails,statistics");
    var channelsUri = $"https://www.googleapis.com/youtube/v3/channels?part={parts}&id={channelId}";
    var channelsRequest = new HttpRequestMessage(HttpMethod.Get, channelsUri);
    channelsRequest.Headers.Add("Authorization", $"Bearer {accessToken}");

    var channelsResponse = await httpClient.SendAsync(channelsRequest);
    var channelsResponseContent = await channelsResponse.Content.ReadAsStringAsync();

    if (channelsResponse.IsSuccessStatusCode is false)
    {
      return (null, $"Failed to get YouTube channel details: {channelsResponseContent}");
    }

    return (channelsResponseContent, null);
  }

  private async Task<(string? result, string? error)> ListYoutubeSubscriptions(string accessToken, string from, int limit)
  {
    var parts = Uri.EscapeDataString("snippet");
    var subscriptionsUri = $"https://www.googleapis.com/youtube/v3/subscriptions?part={parts}&mine=true&order=alphabetical&maxResults={limit}&pageToken={from}";
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

  private async Task<(string? result, string? error)> UnsubscribeYoutubeChannel(string accessToken, string channelId)
  {
    var unsubscribeUri = $"https://www.googleapis.com/youtube/v3/subscriptions?id={channelId}";
    var unsubscribeRequest = new HttpRequestMessage(HttpMethod.Delete, unsubscribeUri);
    unsubscribeRequest.Headers.Add("Authorization", $"Bearer {accessToken}");

    var unsubscribeResponse = await httpClient.SendAsync(unsubscribeRequest);
    var unsubscribeResponseContent = await unsubscribeResponse.Content.ReadAsStringAsync();

    if (unsubscribeResponse.IsSuccessStatusCode is false)
    {
      return (null, $"Failed to unsubscribe YouTube channel: {unsubscribeResponseContent}");
    }

    return (unsubscribeResponseContent, null);
  }

  private async Task<(string? result, string? error)> SubscribeYoutubeChannel(string accessToken, string channelId)
  {
    var subscribeUri = $"https://www.googleapis.com/youtube/v3/subscriptions?part=snippet";
    var subscribeRequest = new HttpRequestMessage(HttpMethod.Post, subscribeUri);
    subscribeRequest.Headers.Add("Authorization", $"Bearer {accessToken}");

    var body = new
    {
      snippet = new
      {
        resourceId = new
        {
          kind = "youtube#channel",
          channelId = channelId
        }
      }
    };

    subscribeRequest.Content = new StringContent(JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");

    var subscribeResponse = await httpClient.SendAsync(subscribeRequest);
    var subscribeResponseContent = await subscribeResponse.Content.ReadAsStringAsync();

    if (subscribeResponse.IsSuccessStatusCode is false)
    {
      return (null, $"Failed to subscribe to YouTube channel: {subscribeResponseContent}");
    }

    return (subscribeResponseContent, null);
  }
}
