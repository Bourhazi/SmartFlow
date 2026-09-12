using Microsoft.AspNetCore.Http;

namespace SmartFlow.Api.Contracts.Requests;

public sealed class UploadAttachmentRequest
{
    public IFormFile? File { get; init; }

    public Guid UploadedById { get; init; }

    public uint Version { get; init; }
}