namespace SmartFlow.Api.Contracts.Requests;

public sealed record StartReviewRequest(
    Guid ManagerId,
    uint Version);