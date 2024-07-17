namespace Models;

public record AuthUser
(
    string UserName,
    string? Name,
    string Role,
    bool IsActive
);
