using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Api.Contracts.Requests;
using SmartFlow.Application.Requests.Commands.CreateRequest;
using SmartFlow.Application.Requests.Queries.GetRequestById;
using SmartFlow.Application.Requests.Commands.UpdateRequest;
using SmartFlow.Application.Requests.Commands.SubmitRequest;
using SmartFlow.Application.Requests.Commands.AssignManager;

namespace SmartFlow.Api.Controllers;

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
}