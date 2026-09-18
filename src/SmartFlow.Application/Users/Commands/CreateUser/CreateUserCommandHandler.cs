using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserAdministrationService userAdministrationService,
    ICurrentUser currentUser,
    IAuditLogger auditLogger)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can create users.");
        }

        var user = await userAdministrationService.CreateAsync(
            command.FullName,
            command.Email,
            command.Password,
            command.Role,
            cancellationToken);

        await auditLogger.WriteAsync(
            "UserCreated",
            "User",
            user.Id,
            null,
            new
            {
                user.FullName,
                user.Email,
                user.Roles,
                user.IsActive
            },
            cancellationToken);

        return user;
    }
}