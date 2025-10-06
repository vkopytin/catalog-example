using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.YoutubeApi;

namespace Controllers;

[Route("youtube-api/[action]")]
[ApiController]
public class YoutubeApiController : ControllerBase
{
  private readonly YoutubeApiService youtubeApi;

  public YoutubeApiController(YoutubeApiService youtubeApi)
  {
    this.youtubeApi = youtubeApi;
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-channels")]
  public IActionResult ListChannels()
  {
    var (result, err) = youtubeApi.ListChannges();

    if (result is null)
    {
      return BadRequest(err);
    }

    return Ok(result);
  }
}