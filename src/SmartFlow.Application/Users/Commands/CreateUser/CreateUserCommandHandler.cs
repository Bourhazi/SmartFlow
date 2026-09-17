using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Users.Commands.CreateUser;

public sealed class CreateUserCommandHandler(
    IUserAdministrationService userAdministrationService,
    ICurrentUser currentUser)
    : IRequestHandler<CreateUserCommand, UserDto>
{
    public Task<UserDto> Handle(
        CreateUserCommand command,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can create users.");
        }

        return userAdministrationService.CreateAsync(
            command.FullName,
            command.Email,
            command.Password,
            command.Role,
            cancellationToken);
    }
}