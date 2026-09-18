using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Refresh;

public sealed class RefreshCommandHandler(
    IAuthenticationService authenticationService)
    : IRequestHandler<RefreshCommand, AuthResult>
{
    public Task<AuthResult> Handle(
        RefreshCommand command,
        CancellationToken cancellationToken)
    {
        return authenticationService.RefreshAsync(
            command.RefreshToken,
            cancellationToken);
    }
}