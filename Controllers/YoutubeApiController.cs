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
  [HttpGet("list-channels")]
  [ActionName("list-channels")]
  public async Task<IActionResult> ListChannels([FromQuery] int from = 0, [FromQuery] int limit = 10)
  {
    var openId = User.GetOid();
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
    await Task.Delay(1);

    return Ok();
  }
}