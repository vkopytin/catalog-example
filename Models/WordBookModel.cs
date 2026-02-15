namespace Models;

public record WordBookModel(
    string Id,
    string En,
    string De,
    DateTime CreatedAt,
    DateTime? UpdatedAt = null
);
