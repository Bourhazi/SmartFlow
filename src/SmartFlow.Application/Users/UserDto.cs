namespace SmartFlow.Application.Users;

public sealed record UserDto(
    Guid Id,
    string FullName,
    string Email,
    bool IsActive,
    IReadOnlyCollection<string> Roles,
    DateTimeOffset? LockoutEnd);