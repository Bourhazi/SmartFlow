using SmartFlow.Domain.Common;
using SmartFlow.Domain.Enums;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Domain.Entities;

public sealed class Request : BaseEntity
{
    private readonly List<Attachment> _attachments = [];
    private readonly List<Comment> _comments = [];
    private readonly List<ApprovalHistory> _approvalHistory = [];

    private Request()
    {
    }

    private Request(
        string title,
        string description,
        RequestPriority priority,
        DateTime? dueDate,
        Guid creatorId)
    {
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate;
        CreatorId = creatorId;
        Status = RequestStatus.Draft;
    }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public RequestStatus Status { get; private set; }

    public RequestPriority Priority { get; private set; }

    public DateTime? DueDate { get; private set; }

    public DateTime? SubmittedAtUtc { get; private set; }

    public DateTime? DecisionAtUtc { get; private set; }

    public Guid CreatorId { get; private set; }

    public Guid? AssignedManagerId { get; private set; }

    public string? RejectionReason { get; private set; }

    public uint Version { get; private set; }

    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    public IReadOnlyCollection<ApprovalHistory> ApprovalHistories =>
        _approvalHistory.AsReadOnly();

    public static Request Create(
        string title,
        string description,
        RequestPriority priority,
        DateTime? dueDate,
        Guid creatorId)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidatePriority(priority);
        ValidateDueDate(dueDate);

        if (creatorId == Guid.Empty)
        {
            throw new DomainException("Creator identifier is required.");
        }

        return new Request(
            title.Trim(),
            description.Trim(),
            priority,
            dueDate,
            creatorId);
    }

    public void Update(
        string title,
        string description,
        RequestPriority priority,
        DateTime? dueDate,
        Guid currentUserId)
    {
        EnsureOwner(currentUserId);
        EnsureDraft();

        ValidateTitle(title);
        ValidateDescription(description);
        ValidatePriority(priority);
        ValidateDueDate(dueDate);

        Title = title.Trim();
        Description = description.Trim();
        Priority = priority;
        DueDate = dueDate;

        MarkAsUpdated();
    }

    public void Submit(Guid currentUserId)
    {
        EnsureOwner(currentUserId);
        EnsureDraft();

        ChangeStatus(RequestStatus.Submitted, currentUserId);

        SubmittedAtUtc = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public void AssignManager(Guid managerId, Guid performedById)
    {
        EnsureNotCompleted();

        if (managerId == Guid.Empty)
        {
            throw new DomainException("Manager identifier is required.");
        }

        if (performedById == Guid.Empty)
        {
            throw new DomainException("The user performing the action is required.");
        }

        AssignedManagerId = managerId;
        MarkAsUpdated();
    }

    public void StartReview(Guid managerId)
    {
        EnsureAssignedManager(managerId);

        if (Status != RequestStatus.Submitted)
        {
            throw new DomainException(
                "Only a submitted request can be moved to review.");
        }

        ChangeStatus(RequestStatus.UnderReview, managerId);
        MarkAsUpdated();
    }

    public void Approve(Guid managerId, string? decisionComment)
    {
        EnsureAssignedManager(managerId);
        EnsureUnderReview();

        ChangeStatus(
            RequestStatus.Approved,
            managerId,
            decisionComment);

        DecisionAtUtc = DateTime.UtcNow;
        RejectionReason = null;

        MarkAsUpdated();
    }

    public void Reject(Guid managerId, string rejectionReason)
    {
        EnsureAssignedManager(managerId);
        EnsureUnderReview();

        if (string.IsNullOrWhiteSpace(rejectionReason))
        {
            throw new DomainException("A rejection reason is required.");
        }

        if (rejectionReason.Trim().Length > 1000)
        {
            throw new DomainException(
                "The rejection reason cannot exceed 1000 characters.");
        }

        RejectionReason = rejectionReason.Trim();

        ChangeStatus(
            RequestStatus.Rejected,
            managerId,
            RejectionReason);

        DecisionAtUtc = DateTime.UtcNow;
        MarkAsUpdated();
    }

    public Attachment AddAttachment(
        string originalFileName,
        string storageFileName,
        string contentType,
        long size,
        Guid uploadedById)
    {
        EnsureNotCompleted();

        var attachment = Attachment.Create(
            Id,
            originalFileName,
            storageFileName,
            contentType,
            size,
            uploadedById);

        _attachments.Add(attachment);
        MarkAsUpdated();

        return attachment;
    }

    public void RemoveAttachment(Guid attachmentId, Guid currentUserId)
    {
        EnsureNotCompleted();

        if (attachmentId == Guid.Empty)
        {
            throw new DomainException("Attachment identifier is required.");
        }

        var attachment = _attachments.SingleOrDefault(x => x.Id == attachmentId);

        if (attachment is null)
        {
            throw new DomainException("Attachment was not found.");
        }

        if (attachment.UploadedById != currentUserId)
        {
            throw new DomainException(
                "Only the attachment uploader can remove it.");
        }

        _attachments.Remove(attachment);
        MarkAsUpdated();
    }

    public Comment AddComment(string content, Guid authorId)
    {
        EnsureNotCompleted();

        var comment = Comment.Create(Id, content, authorId);

        _comments.Add(comment);
        MarkAsUpdated();

        return comment;
    }

    public void UpdateComment(
        Guid commentId,
        string content,
        Guid currentUserId)
    {
        EnsureNotCompleted();

        var comment = GetComment(commentId);
        comment.Update(content, currentUserId);

        MarkAsUpdated();
    }

    public void RemoveComment(Guid commentId, Guid currentUserId)
    {
        EnsureNotCompleted();

        var comment = GetComment(commentId);

        if (comment.AuthorId != currentUserId)
        {
            throw new DomainException(
                "Only the comment author can remove it.");
        }

        _comments.Remove(comment);
        MarkAsUpdated();
    }

    private Comment GetComment(Guid commentId)
    {
        if (commentId == Guid.Empty)
        {
            throw new DomainException("Comment identifier is required.");
        }

        return _comments.SingleOrDefault(x => x.Id == commentId)
            ?? throw new DomainException("Comment was not found.");
    }

    private void ChangeStatus(
        RequestStatus newStatus,
        Guid performedById,
        string? comment = null)
    {
        var history = ApprovalHistory.Create(
            Id,
            Status,
            newStatus,
            performedById,
            comment);

        _approvalHistory.Add(history);
        Status = newStatus;
    }

    private void EnsureOwner(Guid currentUserId)
    {
        if (currentUserId == Guid.Empty || CreatorId != currentUserId)
        {
            throw new DomainException(
                "Only the request creator can perform this action.");
        }
    }

    private void EnsureDraft()
    {
        if (Status != RequestStatus.Draft)
        {
            throw new DomainException(
                "Only a draft request can be modified.");
        }
    }

    private void EnsureUnderReview()
    {
        if (Status != RequestStatus.UnderReview)
        {
            throw new DomainException(
                "Only a request under review can be approved or rejected.");
        }
    }

    private void EnsureAssignedManager(Guid managerId)
    {
        if (managerId == Guid.Empty ||
            !AssignedManagerId.HasValue ||
            AssignedManagerId.Value != managerId)
        {
            throw new DomainException(
                "Only the assigned manager can perform this action.");
        }
    }

    private void EnsureNotCompleted()
    {
        if (Status is RequestStatus.Approved or RequestStatus.Rejected)
        {
            throw new DomainException(
                "A completed request cannot be modified.");
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Request title is required.");
        }

        if (title.Trim().Length > 150)
        {
            throw new DomainException(
                "Request title cannot exceed 150 characters.");
        }
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Request description is required.");
        }

        if (description.Trim().Length > 3000)
        {
            throw new DomainException(
                "Request description cannot exceed 3000 characters.");
        }
    }

    private static void ValidatePriority(RequestPriority priority)
    {
        if (!Enum.IsDefined(priority))
        {
            throw new DomainException("Request priority is invalid.");
        }
    }

    private static void ValidateDueDate(DateTime? dueDate)
    {
        if (dueDate.HasValue && dueDate.Value <= DateTime.UtcNow)
        {
            throw new DomainException("Due date must be in the future.");
        }
    }
}