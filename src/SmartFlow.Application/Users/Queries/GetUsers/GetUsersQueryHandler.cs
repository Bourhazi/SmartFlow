using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler(
    IUserAdministrationService userAdministrationService,
    ICurrentUser currentUser)
    : IRequestHandler<GetUsersQuery, IReadOnlyCollection<UserDto>>
{
    public Task<IReadOnlyCollection<UserDto>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can list users.");
        }

        return userAdministrationService.GetAllAsync(
            query.Search,
            cancellationToken);
    }
}