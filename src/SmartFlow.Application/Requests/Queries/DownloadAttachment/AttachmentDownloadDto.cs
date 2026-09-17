namespace SmartFlow.Application.Requests.Queries.DownloadAttachment;

public sealed record AttachmentDownloadDto(
    string OriginalFileName,
    string ContentType,
    Stream Content);