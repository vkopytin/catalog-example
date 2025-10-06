using System.Text.Json;
using Db.Records;
using Errors;
using Microsoft.AspNetCore.Http.HttpResults;
using Models.YoutubeApi;

namespace Services.YoutubeApi;

public class YoutubeApiService
{
  private readonly HttpClient httpClient;
  private readonly AuthorizationTokensService authorizationTokensService;

  public YoutubeApiService(
    AuthorizationTokensService authorizationTokensService,
    HttpClient httpClient
  )
  {
    this.authorizationTokensService = authorizationTokensService;
    this.httpClient = httpClient;
  }

  public async Task<(YoutubeChannelsResponse? result, ServiceError? error)> ListChannels(string sequrityGroupId)
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

      return (channels, null);
    }
    catch (Exception ex)
    {
      return (null, new ServiceError($"Failed to deserialize YouTube channels response: {ex.Message}"));
    }
  }


  private async Task<(string? result, string? error)> ListYoutubeSubscriptions(string accessToken)
  {
    const string subscriptionsUri = "https://www.googleapis.com/youtube/v3/subscriptions?part=snippet&mine=true";
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
