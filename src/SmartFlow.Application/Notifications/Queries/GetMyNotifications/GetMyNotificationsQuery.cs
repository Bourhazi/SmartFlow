using MediatR;

namespace SmartFlow.Application.Notifications.Queries.GetMyNotifications;

public sealed record GetMyNotificationsQuery
    : IRequest<IReadOnlyCollection<NotificationDto>>;