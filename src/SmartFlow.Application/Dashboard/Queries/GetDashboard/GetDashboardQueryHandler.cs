using MediatR;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Dashboard.Queries.GetDashboard;

public sealed class GetDashboardQueryHandler(
    IDashboardReadService dashboardReadService,
    ICurrentUser currentUser)
    : IRequestHandler<GetDashboardQuery, DashboardDto>
{
    public Task<DashboardDto> Handle(
        GetDashboardQuery query,
        CancellationToken cancellationToken)
    {
        return dashboardReadService.GetAsync(
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