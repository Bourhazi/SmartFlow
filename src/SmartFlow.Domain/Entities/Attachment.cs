using SmartFlow.Domain.Common;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Domain.Entities;

public sealed class Attachment : BaseEntity
{
    private Attachment()
    {
    }

    private Attachment(
        Guid requestId,
        string originalFileName,
        string storageFileName,
        string contentType,
        long size,
        Guid uploadedById)
    {
        RequestId = requestId;
        OriginalFileName = originalFileName;
        StorageFileName = storageFileName;
        ContentType = contentType;
        Size = size;
        UploadedById = uploadedById;
    }

    public Guid RequestId { get; private set; }

    public string OriginalFileName { get; private set; } = string.Empty;

    public string StorageFileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long Size { get; private set; }

    public Guid UploadedById { get; private set; }

    internal static Attachment Create(
        Guid requestId,
        string originalFileName,
        string storageFileName,
        string contentType,
        long size,
        Guid uploadedById)
    {
        if (requestId == Guid.Empty)
        {
            throw new DomainException("Request identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(originalFileName))
        {
            throw new DomainException("Original file name is required.");
        }

        if (string.IsNullOrWhiteSpace(storageFileName))
        {
            throw new DomainException("Storage file name is required.");
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new DomainException("Content type is required.");
        }

        if (size <= 0 || size > 5 * 1024 * 1024)
        {
            throw new DomainException(
                "File size must be between 1 byte and 5 MB.");
        }

        if (uploadedById == Guid.Empty)
        {
            throw new DomainException("Uploader identifier is required.");
        }

        return new Attachment(
            requestId,
            originalFileName.Trim(),
            storageFileName.Trim(),
            contentType.Trim(),
            size,
            uploadedById);
    }
}