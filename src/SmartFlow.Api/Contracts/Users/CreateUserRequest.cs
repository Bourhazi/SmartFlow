namespace SmartFlow.Api.Contracts.Users;

public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    string Role);