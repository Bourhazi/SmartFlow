using SmartFlow.Domain.Enums;

namespace SmartFlow.Api.Contracts.Requests;

public sealed record CreateRequestRequest(
    string Title,
    string Description,
    RequestPriority Priority,
    DateTime? DueDate,
    Guid CreatorId);