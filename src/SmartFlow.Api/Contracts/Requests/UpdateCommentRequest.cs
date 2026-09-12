namespace SmartFlow.Api.Contracts.Requests;

public sealed record UpdateCommentRequest(
    string Content,
    Guid CurrentUserId,
    uint Version);