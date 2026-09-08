using SmartFlow.Domain.Entities;
using SmartFlow.Domain.Enums;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Domain.Tests.Entities;

public sealed class RequestTests
{
    private static readonly Guid CreatorId = Guid.NewGuid();
    private static readonly Guid ManagerId = Guid.NewGuid();
    private static readonly Guid OtherUserId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_CreatesDraftRequest()
    {
        var request = CreateDraft();

        Assert.Equal(RequestStatus.Draft, request.Status);
        Assert.Equal(CreatorId, request.CreatorId);
        Assert.Equal(RequestPriority.Normal, request.Priority);
        Assert.Empty(request.ApprovalHistories);
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Request.Create(
                " ",
                "Description",
                RequestPriority.Normal,
                null,
                CreatorId));
    }

    [Fact]
    public void Create_WithInvalidPriority_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Request.Create(
                "Title",
                "Description",
                (RequestPriority)99,
                null,
                CreatorId));
    }

    [Fact]
    public void Create_WithPastDueDate_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Request.Create(
                "Title",
                "Description",
                RequestPriority.Normal,
                DateTime.UtcNow.AddMinutes(-1),
                CreatorId));
    }

    [Fact]
    public void Update_ByCreatorWhileDraft_UpdatesRequest()
    {
        var request = CreateDraft();

        request.Update(
            "New title",
            "New description",
            RequestPriority.High,
            DateTime.UtcNow.AddDays(7),
            CreatorId);

        Assert.Equal("New title", request.Title);
        Assert.Equal("New description", request.Description);
        Assert.Equal(RequestPriority.High, request.Priority);
        Assert.NotNull(request.UpdatedAtUtc);
    }

    [Fact]
    public void Update_ByAnotherUser_ThrowsDomainException()
    {
        var request = CreateDraft();

        Assert.Throws<DomainException>(() =>
            request.Update(
                "New title",
                "New description",
                RequestPriority.High,
                null,
                OtherUserId));
    }

    [Fact]
    public void Submit_ByCreator_ChangesStatusAndCreatesHistory()
    {
        var request = CreateDraft();

        request.Submit(CreatorId);

        Assert.Equal(RequestStatus.Submitted, request.Status);
        Assert.NotNull(request.SubmittedAtUtc);

        var history = Assert.Single(request.ApprovalHistories);
        Assert.Equal(RequestStatus.Draft, history.OldStatus);
        Assert.Equal(RequestStatus.Submitted, history.NewStatus);
        Assert.Equal(CreatorId, history.PerformedById);
    }

    [Fact]
    public void Approve_BeforeUnderReview_ThrowsDomainException()
    {
        var request = CreateDraft();
        request.Submit(CreatorId);
        request.AssignManager(ManagerId, OtherUserId);

        Assert.Throws<DomainException>(() =>
            request.Approve(ManagerId, "Approved"));
    }

    [Fact]
    public void AssignedManager_CanApproveRequestUnderReview()
    {
        var request = CreateRequestUnderReview();

        request.Approve(ManagerId, "Everything is valid.");

        Assert.Equal(RequestStatus.Approved, request.Status);
        Assert.NotNull(request.DecisionAtUtc);

        var history = request.ApprovalHistories.Last();
        Assert.Equal(RequestStatus.UnderReview, history.OldStatus);
        Assert.Equal(RequestStatus.Approved, history.NewStatus);
        Assert.Equal(ManagerId, history.PerformedById);
    }

    [Fact]
    public void OtherManager_CannotApproveRequest()
    {
        var request = CreateRequestUnderReview();

        Assert.Throws<DomainException>(() =>
            request.Approve(OtherUserId, "Approved"));
    }

    [Fact]
    public void Reject_WithoutReason_ThrowsDomainException()
    {
        var request = CreateRequestUnderReview();

        Assert.Throws<DomainException>(() =>
            request.Reject(ManagerId, " "));
    }

    [Fact]
    public void RejectedRequest_CannotReceiveComment()
    {
        var request = CreateRequestUnderReview();
        request.Reject(ManagerId, "Missing document.");

        Assert.Throws<DomainException>(() =>
            request.AddComment("Why was this rejected?", CreatorId));
    }

    [Fact]
    public void CommentAuthor_CanUpdateComment()
    {
        var request = CreateDraft();
        var comment = request.AddComment("First comment", CreatorId);

        request.UpdateComment(comment.Id, "Updated comment", CreatorId);

        Assert.Equal("Updated comment", comment.Content);
        Assert.NotNull(comment.UpdatedAtUtc);
    }

    [Fact]
    public void AnotherUser_CannotUpdateComment()
    {
        var request = CreateDraft();
        var comment = request.AddComment("First comment", CreatorId);

        Assert.Throws<DomainException>(() =>
            request.UpdateComment(comment.Id, "Updated comment", OtherUserId));
    }

    [Fact]
    public void AttachmentUploader_CanRemoveAttachment()
    {
        var request = CreateDraft();

        var attachment = request.AddAttachment(
            "budget.pdf",
            "3f307ac2.pdf",
            "application/pdf",
            1_024,
            CreatorId);

        request.RemoveAttachment(attachment.Id, CreatorId);

        Assert.Empty(request.Attachments);
    }

    [Fact]
    public void CompletedRequest_CannotBeReassigned()
    {
        var request = CreateRequestUnderReview();
        request.Approve(ManagerId, "Approved");

        Assert.Throws<DomainException>(() =>
            request.AssignManager(Guid.NewGuid(), OtherUserId));
    }

    private static Request CreateDraft()
    {
        return Request.Create(
            "Purchase of a laptop",
            "A new laptop is needed for the development team.",
            RequestPriority.Normal,
            DateTime.UtcNow.AddDays(7),
            CreatorId);
    }

    private static Request CreateRequestUnderReview()
    {
        var request = CreateDraft();

        request.Submit(CreatorId);
        request.AssignManager(ManagerId, OtherUserId);
        request.StartReview(ManagerId);

        return request;
    }
}