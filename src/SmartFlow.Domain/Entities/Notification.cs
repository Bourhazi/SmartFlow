using SmartFlow.Domain.Common;
using SmartFlow.Domain.Enums;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Domain.Entities;

public sealed class Notification : BaseEntity
{
    public string Title { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public NotificationType Type { get; private set; }

    public bool IsRead { get; private set; }

    public Guid UserId { get; private set; }

    public Guid? RequestId { get; private set; }

    private Notification()
    {
    }

    public static Notification Create(
        string title,
        string message,
        NotificationType type,
        Guid userId,
        Guid? requestId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Notification title is required.");
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new DomainException("Notification message is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException(
                "Notification recipient is required.");
        }

        return new Notification
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Message = message.Trim(),
            Type = type,
            UserId = userId,
            RequestId = requestId,
            IsRead = false,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkAsRead()
    {
        if (IsRead)
        {
            return;
        }

        IsRead = true;
        MarkAsUpdated();
    }
}