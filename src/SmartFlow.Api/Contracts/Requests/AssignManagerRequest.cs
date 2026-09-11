namespace SmartFlow.Api.Contracts.Requests;

public sealed record AssignManagerRequest(
    Guid ManagerId,
    Guid PerformedById,
    uint Version);