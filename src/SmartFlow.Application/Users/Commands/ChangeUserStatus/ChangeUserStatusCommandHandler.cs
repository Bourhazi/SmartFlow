using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Users.Commands.ChangeUserStatus;

public sealed class ChangeUserStatusCommandHandler(
    IUserAdministrationService userAdministrationService,
    ICurrentUser currentUser)
    : IRequestHandler<ChangeUserStatusCommand, UserDto?>
{
    public Task<UserDto?> Handle(
        ChangeUserStatusCommand command,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can change user status.");
        }

        if (command.UserId == currentUser.UserId &&
            !command.IsActive)
        {
            throw new InvalidOperationException(
                "You cannot deactivate your own administrator account.");
        }

        return userAdministrationService.ChangeStatusAsync(
            command.UserId,
            command.IsActive,
            cancellationToken);
    }
}