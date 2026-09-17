using SmartFlow.Domain.Enums;

namespace SmartFlow.Api.Contracts.Requests;

public sealed record UpdateRequestRequest(
    string Title,
    string Description,
    RequestPriority Priority,
    DateTime? DueDate,
    uint Version);