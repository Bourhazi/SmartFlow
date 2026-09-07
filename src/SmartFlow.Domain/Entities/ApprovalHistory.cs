using SmartFlow.Domain.Common;
using SmartFlow.Domain.Enums;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Domain.Entities;

public sealed class ApprovalHistory : BaseEntity
{
    private ApprovalHistory()
    {
    }

    private ApprovalHistory(
        Guid requestId,
        RequestStatus oldStatus,
        RequestStatus newStatus,
        Guid performedById,
        string? comment)
    {
        RequestId = requestId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        PerformedById = performedById;
        Comment = comment;
    }

    public Guid RequestId { get; private set; }

    public RequestStatus OldStatus { get; private set; }

    public RequestStatus NewStatus { get; private set; }

    public Guid PerformedById { get; private set; }

    public string? Comment { get; private set; }

    internal static ApprovalHistory Create(
        Guid requestId,
        RequestStatus oldStatus,
        RequestStatus newStatus,
        Guid performedById,
        string? comment)
    {
        if (requestId == Guid.Empty)
        {
            throw new DomainException("Request identifier is required.");
        }

        if (performedById == Guid.Empty)
        {
            throw new DomainException(
                "The user performing the action is required.");
        }

        return new ApprovalHistory(
            requestId,
            oldStatus,
            newStatus,
            performedById,
            comment?.Trim());
    }
}