using MediatR;

namespace SmartFlow.Application.Requests.Commands.RemoveAttachment;

public sealed record RemoveAttachmentCommand(
    Guid RequestId,
    Guid AttachmentId,
    Guid CurrentUserId,
    uint Version) : IRequest<bool>;