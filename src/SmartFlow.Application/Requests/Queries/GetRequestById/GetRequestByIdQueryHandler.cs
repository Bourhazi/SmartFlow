using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Requests.Queries.GetRequestById;

public sealed class GetRequestByIdQueryHandler(
    IRequestRepository requestRepository)
    : IRequestHandler<GetRequestByIdQuery, RequestDetailsDto?>
{
    public async Task<RequestDetailsDto?> Handle(
        GetRequestByIdQuery query,
        CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetByIdAsync(
            query.RequestId,
            cancellationToken);

        return request is null ? null : MapToDto(request);
    }

    private static RequestDetailsDto MapToDto(Request request)
    {
        return new RequestDetailsDto(
            request.Id,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            request.SubmittedAtUtc,
            request.DecisionAtUtc,
            request.CreatorId,
            request.AssignedManagerId,
            request.RejectionReason,
            request.CreatedAtUtc,
            request.UpdatedAtUtc,
            request.Version,
            request.Attachments
                .Select(attachment => new AttachmentDto(
                    attachment.Id,
                    attachment.OriginalFileName,
                    attachment.ContentType,
                    attachment.Size,
                    attachment.UploadedById,
                    attachment.CreatedAtUtc))
                .ToList(),
            request.Comments
                .Select(comment => new CommentDto(
                    comment.Id,
                    comment.Content,
                    comment.AuthorId,
                    comment.CreatedAtUtc,
                    comment.UpdatedAtUtc))
                .ToList(),
            request.ApprovalHistories
                .Select(history => new ApprovalHistoryDto(
                    history.Id,
                    history.OldStatus,
                    history.NewStatus,
                    history.PerformedById,
                    history.Comment,
                    history.CreatedAtUtc))
                .ToList());
    }
}