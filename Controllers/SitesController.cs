using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Utils;

namespace Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class SitesController : ControllerBase
{
  private readonly IProfileService profile;
  private readonly IArticlesService articles;
  private readonly IWebSitesService webSites;

  public SitesController(IProfileService profile, IArticlesService articles, IWebSitesService webSites)
  {
    this.profile = profile;
    this.articles = articles;
    this.webSites = webSites;
  }

  [HttpGet]
  [ActionName("public")]
  public IActionResult Public()
  {
    return Ok(new { Test = "test" });
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("profile")]
  public async Task<IActionResult> Profile()
  {
    var securityGroupId = User.GetOid();
    var (profile, err) = await this.profile.GetProfileBySecurityGroupId(securityGroupId);

    if (profile is null)
    {
      return BadRequest(err);
    }

    return Ok(profile);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list")]
  public async Task<IActionResult> ListWebSites(int from = 0, int limit = 20)
  {
    var (webSites, err) = await this.webSites.ListWebSites(from, limit);

    if (webSites is null)
    {
      return BadRequest(err);
    }

    return Ok(webSites);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost]
  [ActionName("select")]
  public async Task<IActionResult> SelectWebSite([FromBody] SelectedWebSiteRequest request)
  {
    var securityGroupId = User.GetOid();
    var (webSite, err) = await this.webSites.SelectWebSite(request.SiteId, securityGroupId);

    if (webSite is null)
    {
      return BadRequest(err);
    }

    return Ok(webSite);
  }

  public record SelectedWebSiteRequest(Guid SiteId);
}
