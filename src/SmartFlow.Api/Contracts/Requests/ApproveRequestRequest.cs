namespace SmartFlow.Api.Contracts.Requests;

public sealed record ApproveRequestRequest(
    Guid ManagerId,
    string? DecisionComment,
    uint Version);