using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Users.Commands.ChangeUserRole;

public sealed class ChangeUserRoleCommandHandler(
    IUserAdministrationService userAdministrationService,
    ICurrentUser currentUser)
    : IRequestHandler<ChangeUserRoleCommand, UserDto?>
{
    public Task<UserDto?> Handle(
        ChangeUserRoleCommand command,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can change user roles.");
        }

        if (!Roles.All.Contains(command.Role))
        {
            throw new InvalidOperationException(
                "Role is invalid.");
        }

        if (command.UserId == currentUser.UserId &&
            command.Role != Roles.Administrateur)
        {
            throw new InvalidOperationException(
                "You cannot remove your own administrator role.");
        }

        return userAdministrationService.ChangeRoleAsync(
            command.UserId,
            command.Role,
            cancellationToken);
    }
}