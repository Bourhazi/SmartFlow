namespace SmartFlow.Api.Contracts.Requests;

public sealed record AddCommentRequest(
    string Content,
    uint Version);