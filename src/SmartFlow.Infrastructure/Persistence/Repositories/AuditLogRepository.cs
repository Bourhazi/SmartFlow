using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository(
    SmartFlowDbContext dbContext)
    : IAuditLogRepository
{
    public async Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken)
    {
        await dbContext.AuditLogs.AddAsync(
            auditLog,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<AuditLog>> GetAsync(
        string? entityType,
        Guid? entityId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        IQueryable<AuditLog> query = dbContext.AuditLogs
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(log =>
                log.EntityType == entityType.Trim());
        }

        if (entityId.HasValue)
        {
            query = query.Where(log =>
                log.EntityId == entityId.Value);
        }

        return await query
            .OrderByDescending(log => log.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}