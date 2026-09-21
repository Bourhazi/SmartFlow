using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Reports;

public sealed record RequestReportItem(
    string Title,
    RequestStatus Status,
    RequestPriority Priority,
    string CreatorEmail,
    string? ManagerEmail,
    DateTime CreatedAtUtc,
    DateTime? DueDate,
    DateTime? DecisionAtUtc,
    string? RejectionReason);