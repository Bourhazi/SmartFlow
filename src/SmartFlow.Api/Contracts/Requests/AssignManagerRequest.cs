namespace SmartFlow.Api.Contracts.Requests;

public sealed record AssignManagerRequest(
    Guid ManagerId,
    uint Version);