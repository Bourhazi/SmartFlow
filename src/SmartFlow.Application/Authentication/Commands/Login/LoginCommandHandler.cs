using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Login;

public sealed class LoginCommandHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        return authenticationService.LoginAsync(
            command.Email,
            command.Password,
            cancellationToken);
    }
}