namespace SmartFlow.Api.Contracts.Requests;

public sealed record ApproveRequestRequest(
    string? DecisionComment,
    uint Version);