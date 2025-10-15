using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models;
using Services;
using Utils;

namespace Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class ArticlesController : ControllerBase
{
  private readonly AuthorizationTokensService authTokens;
  private readonly IProfileService profile;
  private readonly IArticlesService articles;
  private readonly IWebSitesService webSites;
  private readonly ILogger<ArticlesController> logger;

  public ArticlesController(
    AuthorizationTokensService authTokens,
    IProfileService profile,
    IArticlesService articles,
    IWebSitesService webSites,
    ILogger<ArticlesController> logger
  )
  {
    this.authTokens = authTokens;
    this.profile = profile;
    this.articles = articles;
    this.webSites = webSites;
    this.logger = logger;
  }

  [HttpGet]
  [ActionName("public")]
  public IActionResult Public()
  {
    return Ok(new { Test = "test" });
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list")]
  public async Task<IActionResult> ListArticles(int from = 0, int limit = 20)
  {
    var (articles, err) = await this.articles.ListArticles(from, limit);

    if (articles is null)
    {
      return BadRequest(err);
    }

    return Ok(articles);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list-websites")]
  public async Task<IActionResult> ListWebSites(int from = 0, int limit = 20)
  {
    var (webSites, err) = await this.webSites.ListWebSites(from, limit);

    if (webSites is null)
    {
      return BadRequest(err);
    }

    return Ok(webSites);
  }

  [HttpGet]
  [ActionName("{id}")]
  [AllowAnonymous]
  public async Task<IActionResult> GetArticle([FromRoute] Guid id)
  {
    var (article, err) = await this.articles.GetArticle(id);

    if (article is null)
    {
      return BadRequest(err);
    }

    return Ok(article);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost]
  [ActionName("create")]
  public async Task<IActionResult> CreateArticle([FromBody] CreateArticleRequest createArticle)
  {
    ArticleModel article = createArticle.ToArticle();
    var (createdArticle, err) = await this.articles.CreateArticle(article);

    if (createdArticle is null)
    {
      return BadRequest(err);
    }

    var securityGroupId = User.GetOid();
    var (webSite, error) = await this.profile.GetUserWebSite(securityGroupId);
    if (error is not null)
    {
      this.logger.LogWarning("Error getting user website: {ErrorMessage}", error.Message);
    }
    if (webSite is not null)
    {
      await this.profile.PublishArticleToWebSite(createdArticle, webSite);
    }

    return Ok(createdArticle);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPut]
  [ActionName("{id}")]
  public async Task<IActionResult> UpdateArticle(Guid id, [FromBody] ArticleModel article)
  {
    var (updatedArticle, err) = await this.articles.UpdateArticle(id, article);

    if (updatedArticle is null)
    {
      return BadRequest(err);
    }

    return Ok(updatedArticle);
  }
}
