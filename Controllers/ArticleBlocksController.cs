using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Controllers;

[Route("[controller]/[action]")]
[ApiController()]
public class ArticleBlocksController : ControllerBase
{
  private readonly IArticleBlocksService articleBlocks;

  public ArticleBlocksController(IArticleBlocksService articleBlocks)
  {
    this.articleBlocks = articleBlocks;
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpGet]
  [ActionName("list")]
  public async Task<IActionResult> ListArticleBlocks(int from = 0, int limit = 20)
  {
    var (blocks, err) = await this.articleBlocks.ListArticleBlocks(from, limit);

    if (blocks is null)
    {
      return BadRequest(err);
    }

    return Ok(blocks);
  }
}
