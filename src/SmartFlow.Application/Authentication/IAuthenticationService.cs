namespace SmartFlow.Application.Authentication;

public interface IAuthenticationService
{
    Task<AuthResult> RegisterAsync(
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<AuthResult> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken);
}