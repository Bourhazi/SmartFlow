using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Application.AuditLogs;
using SmartFlow.Application.AuditLogs.Queries.GetAuditLogs;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Api.Controllers;

[ApiController]
[Route("api/audit-logs")]
[Authorize(Roles = Roles.Administrateur)]
public sealed class AuditLogsController(ISender sender)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<AuditLogDto>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<AuditLogDto>>> Get(
        [FromQuery] string? entityType,
        [FromQuery] Guid? entityId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var logs = await sender.Send(
            new GetAuditLogsQuery(
                entityType,
                entityId,
                page,
                pageSize),
            cancellationToken);

        return Ok(logs);
    }
}