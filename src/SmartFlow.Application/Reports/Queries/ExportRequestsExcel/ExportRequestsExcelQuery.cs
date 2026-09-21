using MediatR;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Reports.Queries.ExportRequestsExcel;

public sealed record ExportRequestsExcelQuery(
    RequestStatus? Status,
    RequestPriority? Priority,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc)
    : IRequest<ReportFileDto>;