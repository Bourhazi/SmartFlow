using MediatR;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Reports.Queries.ExportRequestsExcel;

public sealed class ExportRequestsExcelQueryHandler(
    IRequestReportService requestReportService,
    ICurrentUser currentUser)
    : IRequestHandler<ExportRequestsExcelQuery, ReportFileDto>
{
    public Task<ReportFileDto> Handle(
        ExportRequestsExcelQuery query,
        CancellationToken cancellationToken)
    {
        return requestReportService.ExportExcelAsync(
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