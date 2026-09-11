namespace SmartFlow.Api.Contracts.Requests;

public sealed record SubmitRequestRequest(
    Guid CurrentUserId,
    uint Version);