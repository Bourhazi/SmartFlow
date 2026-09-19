using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Application.Dashboard;
using SmartFlow.Application.Dashboard.Queries.GetDashboard;
using SmartFlow.Application.Dashboard.Queries.GetRecentRequests;
using SmartFlow.Application.Requests.Queries.GetRequests;

namespace SmartFlow.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController(ISender sender)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<DashboardDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardDto>> Get(
        CancellationToken cancellationToken)
    {
        var dashboard = await sender.Send(
            new GetDashboardQuery(),
            cancellationToken);

        return Ok(dashboard);
    }

    [HttpGet("recent-requests")]
    [ProducesResponseType<IReadOnlyCollection<RequestListItemDto>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<RequestListItemDto>>>
        GetRecentRequests(
            CancellationToken cancellationToken)
    {
        var requests = await sender.Send(
            new GetRecentRequestsQuery(),
            cancellationToken);

        return Ok(requests);
    }
}