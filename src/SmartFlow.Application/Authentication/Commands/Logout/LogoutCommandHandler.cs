using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Logout;

public sealed class LogoutCommandHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(
        LogoutCommand command,
        CancellationToken cancellationToken)
    {
        await authenticationService.LogoutAsync(
            command.RefreshToken,
            cancellationToken);
    }
}