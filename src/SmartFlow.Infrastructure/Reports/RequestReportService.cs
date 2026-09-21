using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartFlow.Application.Reports;
using SmartFlow.Domain.Entities;
using SmartFlow.Infrastructure.Persistence;

namespace SmartFlow.Infrastructure.Reports;

public sealed class RequestReportService(
    SmartFlowDbContext dbContext)
    : IRequestReportService
{
    public async Task<ReportFileDto> ExportExcelAsync(
        ReportScope scope,
        RequestReportFilter filter,
        CancellationToken cancellationToken)
    {
        var requests = (await GetRequestsAsync(
            scope,
            filter,
            cancellationToken)).ToList();

        using var workbook = new XLWorkbook();

        var sheet = workbook.Worksheets.Add("Requests");

        var headers = new[]
        {
            "Title",
            "Status",
            "Priority",
            "Creator",
            "Assigned manager",
            "Created at",
            "Due date",
            "Decision date",
            "Rejection reason"
        };

        for (var index = 0; index < headers.Length; index++)
        {
            sheet.Cell(1, index + 1).Value = headers[index];
        }

        var headerRange = sheet.Range(1, 1, 1, headers.Length);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E3A5F");
        headerRange.Style.Font.FontColor = XLColor.White;

        for (var index = 0; index < requests.Count; index++)
        {
            var request = requests[index];
            var row = index + 2;

            sheet.Cell(row, 1).Value = request.Title;
            sheet.Cell(row, 2).Value = request.Status.ToString();
            sheet.Cell(row, 3).Value = request.Priority.ToString();
            sheet.Cell(row, 4).Value = request.CreatorEmail;
            sheet.Cell(row, 5).Value = request.ManagerEmail ?? string.Empty;
            sheet.Cell(row, 6).Value = request.CreatedAtUtc;
            sheet.Cell(row, 7).Value = request.DueDate;
            sheet.Cell(row, 8).Value = request.DecisionAtUtc;
            sheet.Cell(row, 9).Value = request.RejectionReason ?? string.Empty;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        return new ReportFileDto(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"smartflow-requests-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    public async Task<ReportFileDto> ExportPdfAsync(
        ReportScope scope,
        RequestReportFilter filter,
        CancellationToken cancellationToken)
    {
        var requests = (await GetRequestsAsync(
            scope,
            filter,
            cancellationToken)).ToList();

        var content = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(25);
                page.Size(PageSizes.A4.Landscape());
                
                page.Header()
                    .Text("SmartFlow - Requests report")
                    .FontSize(18)
                    .Bold()
                    .FontColor(Colors.Blue.Darken3);

                page.Content().PaddingVertical(15).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(HeaderStyle).Text("Title");
                        header.Cell().Element(HeaderStyle).Text("Status");
                        header.Cell().Element(HeaderStyle).Text("Priority");
                        header.Cell().Element(HeaderStyle).Text("Creator");
                        header.Cell().Element(HeaderStyle).Text("Manager");
                        header.Cell().Element(HeaderStyle).Text("Due date");
                    });

                    foreach (var request in requests)
                    {
                        table.Cell().Element(CellStyle).Text(request.Title);
                        table.Cell().Element(CellStyle).Text(
                            request.Status.ToString());
                        table.Cell().Element(CellStyle).Text(
                            request.Priority.ToString());
                        table.Cell().Element(CellStyle).Text(
                            request.CreatorEmail);
                        table.Cell().Element(CellStyle).Text(
                            request.ManagerEmail ?? "-");
                        table.Cell().Element(CellStyle).Text(
                            request.DueDate?.ToString("yyyy-MM-dd") ?? "-");
                    }
                });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generated on ");
                        text.Span(DateTime.UtcNow.ToString("u")).Bold();
                    });
            });
        }).GeneratePdf();

        return new ReportFileDto(
            content,
            "application/pdf",
            $"smartflow-requests-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

    private async Task<IReadOnlyCollection<RequestReportItem>>
        GetRequestsAsync(
            ReportScope scope,
            RequestReportFilter filter,
            CancellationToken cancellationToken)
    {
        IQueryable<Request> query = dbContext.Requests
            .AsNoTracking();

        if (!scope.IsAdministrator)
        {
            if (scope.ManagerId.HasValue)
            {
                query = query.Where(request =>
                    request.AssignedManagerId == scope.ManagerId.Value);
            }
            else
            {
                query = query.Where(request =>
                    request.CreatorId == scope.CreatorId!.Value);
            }
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(request =>
                request.Status == filter.Status.Value);
        }

        if (filter.Priority.HasValue)
        {
            query = query.Where(request =>
                request.Priority == filter.Priority.Value);
        }

        if (filter.CreatedFromUtc.HasValue)
        {
            query = query.Where(request =>
                request.CreatedAtUtc >= filter.CreatedFromUtc.Value);
        }

        if (filter.CreatedToUtc.HasValue)
        {
            query = query.Where(request =>
                request.CreatedAtUtc <= filter.CreatedToUtc.Value);
        }

        var requests = await query
            .OrderByDescending(request => request.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var userIds = requests
            .Select(request => request.CreatorId)
            .Concat(requests
                .Where(request => request.AssignedManagerId.HasValue)
                .Select(request => request.AssignedManagerId!.Value))
            .Distinct()
            .ToArray();

        var userEmails = await dbContext.Users
            .Where(user => userIds.Contains(user.Id))
            .ToDictionaryAsync(
                user => user.Id,
                user => user.Email ?? "Unknown",
                cancellationToken);

        return requests.Select(request => new RequestReportItem(
            request.Title,
            request.Status,
            request.Priority,
            userEmails.GetValueOrDefault(
                request.CreatorId,
                "Unknown"),
            request.AssignedManagerId.HasValue
                ? userEmails.GetValueOrDefault(
                    request.AssignedManagerId.Value,
                    "Unknown")
                : null,
            request.CreatedAtUtc,
            request.DueDate,
            request.DecisionAtUtc,
            request.RejectionReason))
            .ToArray();
    }

    private static IContainer HeaderStyle(IContainer container)
    {
        return container
            .Background(Colors.Blue.Darken3)
            .Padding(5)
            .DefaultTextStyle(style =>
                style.FontColor(Colors.White).Bold());
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(4);
    }
}