using MediatR;

namespace SmartFlow.Application.Requests.Commands.RemoveAttachment;

public sealed record RemoveAttachmentCommand(
    Guid RequestId,
    Guid AttachmentId,
    uint Version) : IRequest<bool>;