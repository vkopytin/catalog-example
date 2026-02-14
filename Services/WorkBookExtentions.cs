using Db.Records;
using Models;

namespace Services;

public static class WordBookExtentions
{
    public static WordBookModel ToModel(this WordBookRecord wordBook)
    {
        return new(
          Id: wordBook.Id,
          En: wordBook.En,
          De: wordBook.De,
          CreatedAt: wordBook.CreatedAt
        );
    }
}
