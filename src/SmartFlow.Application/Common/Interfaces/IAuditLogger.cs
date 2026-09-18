namespace SmartFlow.Application.Common.Interfaces;

public interface IAuditLogger
{
    Task WriteAsync(
        string action,
        string entityType,
        Guid entityId,
        object? oldValues,
        object? newValues,
        CancellationToken cancellationToken);
}