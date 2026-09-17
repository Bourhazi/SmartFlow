using SmartFlow.Application.Common.Models;
using SmartFlow.Application.Requests.Queries.GetRequests;

namespace SmartFlow.Application.Common.Interfaces;

public interface IRequestReadService
{
    Task<PagedResult<RequestListItemDto>> GetPagedAsync(
        GetRequestsQuery query,
        CancellationToken cancellationToken);
}