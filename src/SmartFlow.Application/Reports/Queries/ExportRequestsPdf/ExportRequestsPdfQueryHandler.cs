using MediatR;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Reports.Queries.ExportRequestsPdf;

public sealed class ExportRequestsPdfQueryHandler(
    IRequestReportService requestReportService,
    ICurrentUser currentUser)
    : IRequestHandler<ExportRequestsPdfQuery, ReportFileDto>
{
    public Task<ReportFileDto> Handle(
        ExportRequestsPdfQuery query,
        CancellationToken cancellationToken)
    {
        return requestReportService.ExportPdfAsync(
            CreateScope(),
            new RequestReportFilter(
                query.Status,
                query.Priority,
                query.CreatedFromUtc,
                query.CreatedToUtc),
            cancellationToken);
    }

    private ReportScope CreateScope()
    {
        if (currentUser.IsInRole(Roles.Administrateur))
        {
            return new ReportScope(null, null, true);
        }

        if (currentUser.IsInRole(Roles.Manager))
        {
            return new ReportScope(
                null,
                currentUser.UserId,
                false);
        }

        return new ReportScope(
            currentUser.UserId,
            null,
            false);
    }
}