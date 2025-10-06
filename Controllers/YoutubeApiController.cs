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
  public async Task<IActionResult> ListChannels()
  {
    var openId = User.GetOid();
    var (result, err) = await youtubeApi.ListChannels(openId);

    if (result is null)
    {
      return BadRequest(err);
    }

    return Ok(result);
  }
}