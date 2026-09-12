namespace SmartFlow.Api.Contracts.Requests;

public sealed record RemoveCommentRequest(
    Guid CurrentUserId,
    uint Version);