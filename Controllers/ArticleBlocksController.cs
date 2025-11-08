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
  [HttpGet]
  [ActionName("{id}")]
  public async Task<IActionResult> GetBlockById([FromRoute] int id)
  {
    var (block, err) = await articleBlocks.GetArticleBlocById(id);

    if (block is null)
    {
      return NotFound(err);
    }

    return Ok(block);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPost]
  [ActionName("create")]
  public async Task<IActionResult> CreateArticleBlock([FromBody] Models.ArticleBlockModel block)
  {
    var (created, err) = await articleBlocks.CreateArticleBlock(block);

    if (created is null)
    {
      return BadRequest(err);
    }

    return Created($"{created.Id}", created);
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [HttpPut]
  [ActionName("{id}")]
  public async Task<IActionResult> UpdateArticleBlock([FromRoute] int id, [FromBody] Models.ArticleBlockModel block)
  {
    var (updatedBlock, err) = await articleBlocks.UpdateArticleBlock(id, block);

    if (updatedBlock is null)
    {
      return BadRequest(err);
    }

    return Ok(updatedBlock);
  }
}
