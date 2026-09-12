using MediatR;
using SmartFlow.Application.Common.Models;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Requests.Queries.GetRequests;

public sealed record GetRequestsQuery(
    int Page,
    int PageSize,
    RequestStatus? Status,
    RequestPriority? Priority,
    Guid? CreatorId,
    Guid? ManagerId,
    string? Search,
    RequestSortBy SortBy,
    SortDirection SortDirection)
    : IRequest<PagedResult<RequestListItemDto>>;

public enum RequestSortBy
{
    CreatedAt = 1,
    Title = 2,
    DueDate = 3,
    Priority = 4,
    Status = 5
}

public enum SortDirection
{
    Asc = 1,
    Desc = 2
}