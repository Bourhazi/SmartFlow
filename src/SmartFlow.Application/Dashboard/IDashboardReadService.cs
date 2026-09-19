using SmartFlow.Application.Requests.Queries.GetRequests;

namespace SmartFlow.Application.Dashboard;

public interface IDashboardReadService
{
    Task<DashboardDto> GetAsync(
        DashboardScope scope,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<RequestListItemDto>> GetRecentRequestsAsync(
        DashboardScope scope,
        CancellationToken cancellationToken);
}