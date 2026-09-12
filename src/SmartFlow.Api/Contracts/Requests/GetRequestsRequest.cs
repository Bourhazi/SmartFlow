using SmartFlow.Application.Requests.Queries.GetRequests;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Api.Contracts.Requests;

public sealed class GetRequestsRequest
{
    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public RequestStatus? Status { get; init; }

    public RequestPriority? Priority { get; init; }

    public Guid? CreatorId { get; init; }

    public Guid? ManagerId { get; init; }

    public string? Search { get; init; }

    public RequestSortBy SortBy { get; init; }
        = RequestSortBy.CreatedAt;

    public SortDirection SortDirection { get; init; }
        = SortDirection.Desc;
}