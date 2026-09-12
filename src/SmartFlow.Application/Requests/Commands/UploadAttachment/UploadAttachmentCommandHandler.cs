using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.UploadAttachment;

public sealed class UploadAttachmentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    IFileStorage fileStorage)
    : IRequestHandler<UploadAttachmentCommand, Guid?>
{
    public async Task<Guid?> Handle(
        UploadAttachmentCommand command,
        CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetForUpdateAsync(
            command.RequestId,
            command.Version,
            cancellationToken);

        if (request is null)
        {
            return null;
        }

        string? storageFileName = null;

        try
        {
            storageFileName = await fileStorage.SaveAsync(
                command.Content,
                command.OriginalFileName,
                cancellationToken);

            var attachment = request.AddAttachment(
                command.OriginalFileName,
                storageFileName,
                command.ContentType,
                command.Size,
                command.UploadedById);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return attachment.Id;
        }
        catch
        {
            if (storageFileName is not null)
            {
                try
                {
                    await fileStorage.DeleteAsync(
                        storageFileName,
                        cancellationToken);
                }
                catch
                {
                    // Le fichier orphelin pourra être nettoyé plus tard.
                }
            }

            throw;
        }
    }
}