using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Users.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(
    IUserAdministrationService userAdministrationService,
    ICurrentUser currentUser)
    : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public Task<UserDto?> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can view a user.");
        }

        return userAdministrationService.GetByIdAsync(
            query.UserId,
            cancellationToken);
    }
}