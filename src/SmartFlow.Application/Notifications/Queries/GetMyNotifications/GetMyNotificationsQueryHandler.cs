using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Notifications.Queries.GetMyNotifications;

public sealed class GetMyNotificationsQueryHandler(
    INotificationRepository notificationRepository,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetMyNotificationsQuery,
        IReadOnlyCollection<NotificationDto>>
{
    public async Task<IReadOnlyCollection<NotificationDto>> Handle(
        GetMyNotificationsQuery query,
        CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetForUserAsync(
            currentUser.UserId,
            cancellationToken);

        return notifications
            .Select(notification => new NotificationDto(
                notification.Id,
                notification.Title,
                notification.Message,
                notification.Type,
                notification.IsRead,
                notification.CreatedAtUtc,
                notification.RequestId))
            .ToArray();
    }
}