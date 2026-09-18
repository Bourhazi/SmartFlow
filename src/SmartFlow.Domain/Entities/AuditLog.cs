using SmartFlow.Domain.Common;
using SmartFlow.Domain.Exceptions;

namespace SmartFlow.Domain.Entities;

public sealed class AuditLog : BaseEntity
{
    public string Action { get; private set; } = string.Empty;

    public string EntityType { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    public string? OldValues { get; private set; }

    public string? NewValues { get; private set; }

    public string? IpAddress { get; private set; }

    public Guid UserId { get; private set; }

    private AuditLog()
    {
    }

    public static AuditLog Create(
        string action,
        string entityType,
        Guid entityId,
        Guid userId,
        string? oldValues = null,
        string? newValues = null,
        string? ipAddress = null)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            throw new DomainException("Audit action is required.");
        }

        if (string.IsNullOrWhiteSpace(entityType))
        {
            throw new DomainException("Audit entity type is required.");
        }

        if (entityId == Guid.Empty)
        {
            throw new DomainException("Audit entity identifier is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException("Audit user identifier is required.");
        }

        return new AuditLog
        {
            Id = Guid.NewGuid(),
            Action = action.Trim(),
            EntityType = entityType.Trim(),
            EntityId = entityId,
            UserId = userId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            CreatedAtUtc = DateTime.UtcNow
        };
    }
}