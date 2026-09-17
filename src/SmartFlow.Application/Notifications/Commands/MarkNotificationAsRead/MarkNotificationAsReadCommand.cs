using MediatR;

namespace SmartFlow.Application.Notifications.Commands.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(
    Guid NotificationId) : IRequest<bool>;