using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Register;

public sealed class RegisterCommandHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<RegisterCommand, AuthResult>
{
    public Task<AuthResult> Handle(
        RegisterCommand command,
        CancellationToken cancellationToken)
    {
        return authenticationService.RegisterAsync(
            command.FullName,
            command.Email,
            command.Password,
            cancellationToken);
    }
}