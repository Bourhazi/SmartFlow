namespace SmartFlow.Application.AuditLogs;

public sealed record AuditLogDto(
    Guid Id,
    string Action,
    string EntityType,
    Guid EntityId,
    string? OldValues,
    string? NewValues,
    string? IpAddress,
    Guid UserId,
    DateTime CreatedAtUtc);