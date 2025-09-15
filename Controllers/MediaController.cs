using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers;

[Route("[controller]/[action]/{id?}")]
[ApiController]
public class MediaController : ControllerBase
{
  HttpClient client;
  JobsOnDemand jobs;
  readonly IMediaLibraryService mediaLibrary;

  public MediaController(IMediaLibraryService mediaLibrary, HttpClient client, JobsOnDemand jobs)
  {
    this.client = client;
    this.jobs = jobs;
    this.mediaLibrary = mediaLibrary;
  }

  [HttpPost("upload")]
  [ActionName("block")]
  public async Task<IActionResult> BlockUpload([FromRoute(Name = "id")] int blockId, IFormCollection formData)
  {
    var file = formData.Files[0];
    using Stream m = file.OpenReadStream();
    var (res, error) = await this.mediaLibrary.Create(blockId, m, file.ContentType, file.FileName);
    if (res is null)
    {
      return BadRequest(new { Error = error?.Message });
    }

    using var client = new HttpClient();
    var byteArray = Encoding.ASCII.GetBytes($"{jobs.JobUserName}:{jobs.JobPassword}");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

    var response = await client.PostAsync(jobs.TriggerUrl, null);
    if (response.IsSuccessStatusCode)
    {
      return Ok(new { result = res, Message = "WebJob triggered successfully." });
    }
    else
    {
      return BadRequest(new { Error = $"Failed to trigger WebJob. Status code: {response.StatusCode}" });
    }
  }
}
