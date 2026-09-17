namespace SmartFlow.Api.Contracts.Requests;

public sealed record UpdateCommentRequest(
    string Content,
    uint Version);