using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Application.Notifications;
using SmartFlow.Application.Notifications.Commands.MarkNotificationAsRead;
using SmartFlow.Application.Notifications.Queries.GetMyNotifications;

namespace SmartFlow.Api.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController(ISender sender)
    : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<NotificationDto>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<NotificationDto>>> GetMy(
        CancellationToken cancellationToken)
    {
        var notifications = await sender.Send(
            new GetMyNotificationsQuery(),
            cancellationToken);

        return Ok(notifications);
    }

    [HttpPut("{notificationId:guid}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var wasUpdated = await sender.Send(
            new MarkNotificationAsReadCommand(notificationId),
            cancellationToken);

        return wasUpdated ? NoContent() : NotFound();
    }
}