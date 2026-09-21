using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Application.Reports.Queries.ExportRequestsExcel;
using SmartFlow.Application.Reports.Queries.ExportRequestsPdf;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Api.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public sealed class ReportsController(ISender sender)
    : ControllerBase
{
    [HttpGet("requests/excel")]
    public async Task<IActionResult> ExportExcel(
        [FromQuery] RequestStatus? status,
        [FromQuery] RequestPriority? priority,
        [FromQuery] DateTime? createdFromUtc,
        [FromQuery] DateTime? createdToUtc,
        CancellationToken cancellationToken)
    {
        var report = await sender.Send(
            new ExportRequestsExcelQuery(
                status,
                priority,
                createdFromUtc,
                createdToUtc),
            cancellationToken);

        return File(
            report.Content,
            report.ContentType,
            report.FileName);
    }

    [HttpGet("requests/pdf")]
    public async Task<IActionResult> ExportPdf(
        [FromQuery] RequestStatus? status,
        [FromQuery] RequestPriority? priority,
        [FromQuery] DateTime? createdFromUtc,
        [FromQuery] DateTime? createdToUtc,
        CancellationToken cancellationToken)
    {
        var report = await sender.Send(
            new ExportRequestsPdfQuery(
                status,
                priority,
                createdFromUtc,
                createdToUtc),
            cancellationToken);

        return File(
            report.Content,
            report.ContentType,
            report.FileName);
    }
}