namespace Db;

public record PagedResult<T>(
  IEnumerable<T> Items,
  long Total
);
