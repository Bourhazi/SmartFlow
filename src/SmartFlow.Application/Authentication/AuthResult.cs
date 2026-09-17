namespace SmartFlow.Application.Authentication;

public sealed record AuthResult(
    Guid UserId,
    string FullName,
    string Email,
    IReadOnlyCollection<string> Roles,
    string AccessToken,
    DateTime ExpiresAtUtc);