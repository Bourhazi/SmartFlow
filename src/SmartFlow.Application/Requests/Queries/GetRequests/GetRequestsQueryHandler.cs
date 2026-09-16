using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Models;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Queries.GetRequests;

public sealed class GetRequestsQueryHandler(
    IRequestReadService requestReadService,
    ICurrentUser currentUser)
    : IRequestHandler<GetRequestsQuery, PagedResult<RequestListItemDto>>
{
    public Task<PagedResult<RequestListItemDto>> Handle(
        GetRequestsQuery query,
        CancellationToken cancellationToken)
    {
        var securedQuery = ApplyAccessScope(query);

        return requestReadService.GetPagedAsync(
            securedQuery,
            cancellationToken);
    }

    private GetRequestsQuery ApplyAccessScope(
        GetRequestsQuery query)
    {
        if (currentUser.IsInRole(Roles.Administrateur))
        {
            return query;
        }

        if (currentUser.IsInRole(Roles.Manager))
        {
            return query with
            {
                CreatorId = null,
                ManagerId = currentUser.UserId
            };
        }

        return query with
        {
            CreatorId = currentUser.UserId,
            ManagerId = null
        };
    }
}