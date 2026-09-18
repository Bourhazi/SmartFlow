using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Common.Interfaces;

public interface IAuditLogRepository
{
    Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<AuditLog>> GetAsync(
        string? entityType,
        Guid? entityId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}