using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Api.Contracts.Requests;
using SmartFlow.Application.Requests.Commands.CreateRequest;
using SmartFlow.Application.Requests.Queries.GetRequestById;
using SmartFlow.Application.Requests.Commands.UpdateRequest;
using SmartFlow.Application.Requests.Commands.SubmitRequest;
using SmartFlow.Application.Requests.Commands.AssignManager;
using SmartFlow.Application.Requests.Commands.StartReview;
namespace SmartFlow.Api.Controllers;
using SmartFlow.Application.Requests.Commands.ApproveRequest;
using SmartFlow.Application.Requests.Commands.RejectRequest;
using SmartFlow.Application.Requests.Commands.AddComment;
using SmartFlow.Application.Requests.Commands.UpdateComment;
using SmartFlow.Application.Requests.Commands.RemoveComment;
using SmartFlow.Application.Requests.Commands.UploadAttachment;
using SmartFlow.Application.Requests.Commands.RemoveAttachment;
using SmartFlow.Application.Requests.Queries.DownloadAttachment;
using SmartFlow.Application.Common.Models;
using SmartFlow.Application.Requests.Queries.GetRequests;


[ApiController]
[Route("api/requests")]
public sealed class RequestsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(
        CreateRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateRequestCommand(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CreatorId);

        var requestId = await sender.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { requestId },
            new { id = requestId });
    }



    [HttpPut("{requestId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid requestId,
        UpdateRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateRequestCommand(
            requestId,
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            request.CurrentUserId,
            request.Version);

        var wasUpdated = await sender.Send(command, cancellationToken);

        return wasUpdated ? NoContent() : NotFound();
    }


    [HttpGet]
    [ProducesResponseType<PagedResult<RequestListItemDto>>(
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<RequestListItemDto>>> GetList(
        [FromQuery] GetRequestsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetRequestsQuery(
            request.Page,
            request.PageSize,
            request.Status,
            request.Priority,
            request.CreatorId,
            request.ManagerId,
            request.Search,
            request.SortBy,
            request.SortDirection);

        var result = await sender.Send(query, cancellationToken);

        return Ok(result);
    }




    [HttpGet("{requestId:guid}")]
    [ProducesResponseType<RequestDetailsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestDetailsDto>> GetById(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        var query = new GetRequestByIdQuery(requestId);

        var request = await sender.Send(query, cancellationToken);

        return request is null ? NotFound() : Ok(request);
    }



    [HttpPost("{requestId:guid}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Submit(
        Guid requestId,
        SubmitRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SubmitRequestCommand(
            requestId,
            request.CurrentUserId,
            request.Version);

        var wasSubmitted = await sender.Send(command, cancellationToken);

        return wasSubmitted ? NoContent() : NotFound();
    }



    [HttpPost("{requestId:guid}/assign-manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignManager(
        Guid requestId,
        AssignManagerRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignManagerCommand(
            requestId,
            request.ManagerId,
            request.PerformedById,
            request.Version);

        var wasAssigned = await sender.Send(command, cancellationToken);

        return wasAssigned ? NoContent() : NotFound();
    }

    
    [HttpPost("{requestId:guid}/start-review")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StartReview(
        Guid requestId,
        StartReviewRequest request,
        CancellationToken cancellationToken)
    {
        var command = new StartReviewCommand(
            requestId,
            request.ManagerId,
            request.Version);

        var wasStarted = await sender.Send(command, cancellationToken);

        return wasStarted ? NoContent() : NotFound();
    }


    [HttpPost("{requestId:guid}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Approve(
        Guid requestId,
        ApproveRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ApproveRequestCommand(
            requestId,
            request.ManagerId,
            request.DecisionComment,
            request.Version);

        var wasApproved = await sender.Send(command, cancellationToken);

        return wasApproved ? NoContent() : NotFound();
    }

    [HttpPost("{requestId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Reject(
        Guid requestId,
        RejectRequestRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RejectRequestCommand(
            requestId,
            request.ManagerId,
            request.RejectionReason,
            request.Version);

        var wasRejected = await sender.Send(command, cancellationToken);

        return wasRejected ? NoContent() : NotFound();
    }


    [HttpPost("{requestId:guid}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddComment(
        Guid requestId,
        AddCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCommentCommand(
            requestId,
            request.Content,
            request.AuthorId,
            request.Version);

        var commentId = await sender.Send(command, cancellationToken);

        return commentId.HasValue
            ? Created(
                $"/api/requests/{requestId}",
                new { id = commentId.Value })
            : NotFound();
    }

    [HttpPut("{requestId:guid}/comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateComment(
        Guid requestId,
        Guid commentId,
        UpdateCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCommentCommand(
            requestId,
            commentId,
            request.Content,
            request.CurrentUserId,
            request.Version);

        var wasUpdated = await sender.Send(command, cancellationToken);

        return wasUpdated ? NoContent() : NotFound();
    }

    [HttpDelete("{requestId:guid}/comments/{commentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveComment(
        Guid requestId,
        Guid commentId,
        [FromBody] RemoveCommentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCommentCommand(
            requestId,
            commentId,
            request.CurrentUserId,
            request.Version);

        var wasRemoved = await sender.Send(command, cancellationToken);

        return wasRemoved ? NoContent() : NotFound();
    }


    [HttpPost("{requestId:guid}/attachments")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UploadAttachment(
        Guid requestId,
        [FromForm] UploadAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return BadRequest(new
            {
                title = "Invalid file",
                detail = "A non-empty file is required."
            });
        }

        await using var content = request.File.OpenReadStream();

        var command = new UploadAttachmentCommand(
            requestId,
            request.File.FileName,
            request.File.ContentType,
            request.File.Length,
            content,
            request.UploadedById,
            request.Version);

        var attachmentId = await sender.Send(command, cancellationToken);

        return attachmentId.HasValue
            ? Created(
                $"/api/requests/{requestId}/attachments/{attachmentId.Value}/download",
                new { id = attachmentId.Value })
            : NotFound();
    }

    [HttpGet("{requestId:guid}/attachments/{attachmentId:guid}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachment(
        Guid requestId,
        Guid attachmentId,
        CancellationToken cancellationToken)
    {
        var query = new DownloadAttachmentQuery(
            requestId,
            attachmentId);

        var attachment = await sender.Send(query, cancellationToken);

        return attachment is null
            ? NotFound()
            : File(
                attachment.Content,
                attachment.ContentType,
                attachment.OriginalFileName);
    }

    [HttpDelete("{requestId:guid}/attachments/{attachmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RemoveAttachment(
        Guid requestId,
        Guid attachmentId,
        [FromBody] RemoveAttachmentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAttachmentCommand(
            requestId,
            attachmentId,
            request.CurrentUserId,
            request.Version);

        var wasRemoved = await sender.Send(command, cancellationToken);

        return wasRemoved ? NoContent() : NotFound();
    }
}