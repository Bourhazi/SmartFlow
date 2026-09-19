using MediatR;
using SmartFlow.Application.Requests.Queries.GetRequests;

namespace SmartFlow.Application.Dashboard.Queries.GetRecentRequests;

public sealed record GetRecentRequestsQuery
    : IRequest<IReadOnlyCollection<RequestListItemDto>>;