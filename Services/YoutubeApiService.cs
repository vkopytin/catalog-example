using Errors;
using Models.YoutubeApi;

namespace Services.YoutubeApi;

public class YoutubeApiService
{
  public (YoutubeChannelsResponse? result, ServiceError? error) ListChannges()
  {
    return (null, new ServiceError("Not implemented"));
  }
}
