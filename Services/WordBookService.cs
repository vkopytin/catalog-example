using Db;
using Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using MongoDB.Driver;

namespace Services;

public class WordBookService
{
    private readonly ILogger<WordBookService> logger;
    private readonly MongoDbContext dbContext;

    public WordBookService(MongoDbContext dbContext, ILogger<WordBookService> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<(WordBookModel[]? words, ServiceError? err)> SearchWords(string search = "", int skip = 0, int limit = 20)
    {
        await Task.Delay(1);

        try
        {
            var words = dbContext.WordBooksSearch(search, skip, limit);

            return (words.Select(w => w.ToModel()).ToArray(), null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error, listing words");
            return (null, new(Message: ex.Message));
        }
    }

    public async Task<(WordBookModel? word, ServiceError? err)> AddWord(WordBookModel word)
    {
        try
        {
            var record = new Db.Records.WordBookRecord
            {
                Id = Guid.NewGuid().ToString(),
                En = word.En,
                De = word.De,
                CreatedAt = DateTime.UtcNow
            };

            await dbContext.WordBooks.AddAsync(record);
            await dbContext.SaveChangesAsync();

            return (record.ToModel(), null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error, adding word");
            return (null, new(Message: ex.Message));
        }
    }

    public async Task<(WordBookModel[]? words, ServiceError? err)> AddBatch(string entries)
    {
        try
        {
            var lines = entries.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var records = lines.Select(line =>
            {
                var parts = line.Split(" - ");
                if (parts.Length != 2)
                {
                    throw new FormatException($"Invalid entry format: '{line}'. Expected format: 'en\\tde'.");
                }

                return new Db.Records.WordBookRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    De = parts[0].Trim(),
                    En = parts[1].Trim(),
                    CreatedAt = DateTime.UtcNow
                };
            }).ToList();

            var newRecords = records.Where(r => !dbContext.WordBooks.Any(w => w.En == r.En && w.De == r.De)).ToArray();

            await dbContext.WordBooks.AddRangeAsync(newRecords);
            await dbContext.SaveChangesAsync();

            return (records.Select(r => r.ToModel()).ToArray(), null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error, adding batch of words");
            return (null, new(Message: ex.Message));
        }
    }
}