using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Requests.Queries.GetRequests;

public sealed record RequestListItemDto(
    Guid Id,
    string Title,
    RequestStatus Status,
    RequestPriority Priority,
    DateTime? DueDate,
    Guid CreatorId,
    Guid? AssignedManagerId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);