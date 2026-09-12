namespace SmartFlow.Api.Contracts.Requests;

public sealed record RemoveAttachmentRequest(
    Guid CurrentUserId,
    uint Version);