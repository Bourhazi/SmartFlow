using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Application.Requests.Commands.RemoveAttachment;

public sealed class RemoveAttachmentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    IFileStorage fileStorage)
    : IRequestHandler<RemoveAttachmentCommand, bool>
{
    public async Task<bool> Handle(
        RemoveAttachmentCommand command,
        CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetForUpdateAsync(
            command.RequestId,
            command.Version,
            cancellationToken);

        if (request is null)
        {
            return false;
        }

        var attachment = request.Attachments
            .SingleOrDefault(item => item.Id == command.AttachmentId)
            ?? throw new DomainException("Attachment was not found.");

        request.RemoveAttachment(
            command.AttachmentId,
            command.CurrentUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await fileStorage.DeleteAsync(
            attachment.StorageFileName,
            cancellationToken);

        return true;
    }
}