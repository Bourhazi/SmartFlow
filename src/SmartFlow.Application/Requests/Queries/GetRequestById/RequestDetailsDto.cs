using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Requests.Queries.GetRequestById;

public sealed record RequestDetailsDto(
    Guid Id,
    string Title,
    string Description,
    RequestStatus Status,
    RequestPriority Priority,
    DateTime? DueDate,
    DateTime? SubmittedAtUtc,
    DateTime? DecisionAtUtc,
    Guid CreatorId,
    Guid? AssignedManagerId,
    string? RejectionReason,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc,
    uint Version,
    IReadOnlyCollection<AttachmentDto> Attachments,
    IReadOnlyCollection<CommentDto> Comments,
    IReadOnlyCollection<ApprovalHistoryDto> ApprovalHistories);

public sealed record AttachmentDto(
    Guid Id,
    string OriginalFileName,
    string ContentType,
    long Size,
    Guid UploadedById,
    DateTime CreatedAtUtc);

public sealed record CommentDto(
    Guid Id,
    string Content,
    Guid AuthorId,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

public sealed record ApprovalHistoryDto(
    Guid Id,
    RequestStatus OldStatus,
    RequestStatus NewStatus,
    Guid PerformedById,
    string? Comment,
    DateTime CreatedAtUtc);