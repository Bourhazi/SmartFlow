namespace SmartFlow.Api.Contracts.Requests;

public sealed record AddCommentRequest(
    string Content,
    Guid AuthorId,
    uint Version);