using MediatR;

namespace SmartFlow.Application.Requests.Commands.UploadAttachment;

public sealed record UploadAttachmentCommand(
    Guid RequestId,
    string OriginalFileName,
    string ContentType,
    long Size,
    Stream Content,
    Guid UploadedById,
    uint Version) : IRequest<Guid?>;