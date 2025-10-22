using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.YoutubeApi;
using Utils;

namespace Controllers;

[Route("youtube-api")]
[Route("[controller]/[action]")]
[ApiController]
public class YoutubeApiController : ControllerBase
{
  private readonly YoutubeApiService youtubeApi;

  public YoutubeApiController(YoutubeApiService youtubeApi)
  {
    this.youtubeApi = youtubeApi;
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet("list-subscriptions")]
  [ActionName("list-subscriptions")]
  public async Task<IActionResult> ListSubscriptions([FromQuery] int from = 0, [FromQuery] int limit = 10)
  {
    var openId = User.TryGetOid();
    var (result, err) = await youtubeApi.ListSubscriptions(openId, from, limit);

    if (result is null)
    {
      return BadRequest(err);
    }

    return Ok(result);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet("list-channels")]
  [ActionName("list-channels")]
  public async Task<IActionResult> ListChannels([FromQuery] int from = 0, [FromQuery] int limit = 10)
  {
    var openId = User.TryGetOid();
    var (result, err) = await youtubeApi.ListChannels(openId, from, limit);

    if (result is null)
    {
      return BadRequest(err);
    }

    return Ok(result);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost("unsubscribe/{channelId}")]
  [ActionName("unsubscribe/[channelId]")]
  public async Task<IActionResult> Unsubscribe(string channelId)
  {
    var openId = User.TryGetOid();
    var (result, err) = await youtubeApi.UnsubscribeChannel(openId, channelId);

    if (result is null)
    {
      return BadRequest(err);
    }

    return Ok(result);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost("subscribe/{channelId}")]
  [ActionName("subscribe/[channelId]")]
  public async Task<IActionResult> Subscribe(string channelId)
  {
    var openId = User.TryGetOid();

    var (result, err) = await youtubeApi.SubscribeChannel(openId, channelId);

    if (result is null)
    {
      return BadRequest(err);
    }

    return Ok(result);
  }
}