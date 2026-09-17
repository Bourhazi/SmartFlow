using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Queries.DownloadAttachment;

public sealed class DownloadAttachmentQueryHandler(
    IRequestRepository requestRepository,
    IFileStorage fileStorage,
    RequestAccessGuard requestAccessGuard)
    : IRequestHandler<DownloadAttachmentQuery, AttachmentDownloadDto?>
{
    public async Task<AttachmentDownloadDto?> Handle(
        DownloadAttachmentQuery query,
        CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetByIdAsync(
            query.RequestId,
            cancellationToken);

        if (request is null)
        {
            return null;
        }

        requestAccessGuard.EnsureCanRead(
            request.CreatorId,
            request.AssignedManagerId);

        var attachment = request.Attachments
            .SingleOrDefault(item => item.Id == query.AttachmentId);

        if (attachment is null)
        {
            return null;
        }

        var content = await fileStorage.OpenReadAsync(
            attachment.StorageFileName,
            cancellationToken);

        return content is null
            ? null
            : new AttachmentDownloadDto(
                attachment.OriginalFileName,
                attachment.ContentType,
                content);
    }
}