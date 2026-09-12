using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Models;

namespace SmartFlow.Application.Requests.Queries.GetRequests;

public sealed class GetRequestsQueryHandler(
    IRequestReadService requestReadService)
    : IRequestHandler<GetRequestsQuery, PagedResult<RequestListItemDto>>
{
    public Task<PagedResult<RequestListItemDto>> Handle(
        GetRequestsQuery query,
        CancellationToken cancellationToken)
    {
        return requestReadService.GetPagedAsync(
            query,
            cancellationToken);
    }
}