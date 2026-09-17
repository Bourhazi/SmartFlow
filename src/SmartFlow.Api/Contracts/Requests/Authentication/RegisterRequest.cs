namespace SmartFlow.Api.Contracts.Authentication;

public sealed class RegisterRequest
{
    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}