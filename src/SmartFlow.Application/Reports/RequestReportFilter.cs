using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Reports;

public sealed record RequestReportFilter(
    RequestStatus? Status,
    RequestPriority? Priority,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc);