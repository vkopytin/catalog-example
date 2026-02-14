using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services;

namespace Controllers;

[Route("[controller]/[action]")]
[ApiController()]
public class WordBookController : ControllerBase
{
    private readonly WordBookService wordBookService;

    public WordBookController(WordBookService wordBookService)
    {
        this.wordBookService = wordBookService;
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [ActionName("search")]
    public async Task<IActionResult> SearchEntries([FromQuery] string term = "", int from = 0, int limit = 20)
    {
        var (words, err) = await wordBookService.SearchWords(term, from, limit);
        if (err != null)
        {
            return BadRequest(err);
        }
        return Ok(words);
    }

    public record AddEntryRequest(string En, string De);
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [ActionName("add")]
    public async Task<IActionResult> CreateEntry([FromBody] AddEntryRequest entry)
    {
        var (word, err) = await wordBookService.AddWord(new WordBookModel(
            Id: string.Empty,
            En: entry.En,
            De: entry.De,
            CreatedAt: DateTime.UtcNow
        ));

        if (err != null)
        {
            return BadRequest(err);
        }

        return Ok(word);
    }

    public record AddBatchRequest(string Entries);
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [ActionName("add-batch")]
    public async Task<IActionResult> AddBatch([FromBody] AddBatchRequest request)
    {
        var (words, err) = await wordBookService.AddBatch(request.Entries);
        if (err != null)
        {
            return BadRequest(err);
        }
        return Ok(words);
    }

}