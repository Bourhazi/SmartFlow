namespace SmartFlow.Application.Reports;

public interface IRequestReportService
{
    Task<ReportFileDto> ExportExcelAsync(
        ReportScope scope,
        RequestReportFilter filter,
        CancellationToken cancellationToken);

    Task<ReportFileDto> ExportPdfAsync(
        ReportScope scope,
        RequestReportFilter filter,
        CancellationToken cancellationToken);
}