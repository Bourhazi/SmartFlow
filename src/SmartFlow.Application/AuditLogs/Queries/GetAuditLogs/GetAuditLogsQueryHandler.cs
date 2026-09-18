using MediatR;
using SmartFlow.Application.Common.Exceptions;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.AuditLogs.Queries.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler(
    IAuditLogRepository auditLogRepository,
    ICurrentUser currentUser)
    : IRequestHandler<
        GetAuditLogsQuery,
        IReadOnlyCollection<AuditLogDto>>
{
    public async Task<IReadOnlyCollection<AuditLogDto>> Handle(
        GetAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        if (!currentUser.IsInRole(Roles.Administrateur))
        {
            throw new ForbiddenAccessException(
                "Only an administrator can view audit logs.");
        }

        var logs = await auditLogRepository.GetAsync(
            query.EntityType,
            query.EntityId,
            query.Page,
            query.PageSize,
            cancellationToken);

        return logs.Select(log => new AuditLogDto(
            log.Id,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.OldValues,
            log.NewValues,
            log.IpAddress,
            log.UserId,
            log.CreatedAtUtc))
            .ToArray();
    }
}