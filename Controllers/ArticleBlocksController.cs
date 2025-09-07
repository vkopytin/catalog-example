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
    var (blocks, err) = await articleBlocks.ListArticleBlocks(from, limit);

    if (blocks is null)
    {
      return BadRequest(err);
    }

    return Ok(blocks);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPut]
  [ActionName("{id}")]
  public async Task<IActionResult> UpdateArticleBlock([FromRoute] Guid id, [FromBody] Models.ArticleBlockModel block)
  {
    var (updatedBlock, err) = await articleBlocks.UpdateArticleBlock(id, block);

    if (updatedBlock is null)
    {
      return BadRequest(err);
    }

    return Ok(updatedBlock);
  }
}
