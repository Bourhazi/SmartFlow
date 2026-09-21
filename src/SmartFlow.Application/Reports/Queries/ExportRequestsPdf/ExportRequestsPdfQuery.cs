using MediatR;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Reports.Queries.ExportRequestsPdf;

public sealed record ExportRequestsPdfQuery(
    RequestStatus? Status,
    RequestPriority? Priority,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc)
    : IRequest<ReportFileDto>;