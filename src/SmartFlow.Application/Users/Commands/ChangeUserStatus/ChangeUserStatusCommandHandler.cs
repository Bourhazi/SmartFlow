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

        return userAdministrationService.ChangeStatusAsync(
            command.UserId,
            command.IsActive,
            cancellationToken);
    }
}