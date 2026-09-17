using MediatR;

namespace SmartFlow.Application.Requests.Queries.DownloadAttachment;

public sealed record DownloadAttachmentQuery(
    Guid RequestId,
    Guid AttachmentId) : IRequest<AttachmentDownloadDto?>;