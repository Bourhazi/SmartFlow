using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Notifications;

public sealed record NotificationDto(
    Guid Id,
    string Title,
    string Message,
    NotificationType Type,
    bool IsRead,
    DateTime CreatedAtUtc,
    Guid? RequestId);