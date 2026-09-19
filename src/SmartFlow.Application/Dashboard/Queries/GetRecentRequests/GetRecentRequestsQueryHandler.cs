using MediatR;
using SmartFlow.Application.Common.Security;
using SmartFlow.Application.Requests.Queries.GetRequests;

namespace SmartFlow.Application.Dashboard.Queries.GetRecentRequests;

public sealed class GetRecentRequestsQueryHandler(
    IDashboardReadService dashboardReadService,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetRecentRequestsQuery,
        IReadOnlyCollection<RequestListItemDto>>
{
    public Task<IReadOnlyCollection<RequestListItemDto>> Handle(
        GetRecentRequestsQuery query,
        CancellationToken cancellationToken)
    {
        return dashboardReadService.GetRecentRequestsAsync(
            CreateScope(),
            cancellationToken);
    }

    private DashboardScope CreateScope()
    {
        if (currentUser.IsInRole(Roles.Administrateur))
        {
            return new DashboardScope(null, null, true);
        }

        if (currentUser.IsInRole(Roles.Manager))
        {
            return new DashboardScope(
                null,
                currentUser.UserId,
                false);
        }

        return new DashboardScope(
            currentUser.UserId,
            null,
            false);
    }
}