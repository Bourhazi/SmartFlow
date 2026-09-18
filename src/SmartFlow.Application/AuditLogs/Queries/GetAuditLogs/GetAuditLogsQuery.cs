using MediatR;

namespace SmartFlow.Application.AuditLogs.Queries.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    string? EntityType,
    Guid? EntityId,
    int Page = 1,
    int PageSize = 50)
    : IRequest<IReadOnlyCollection<AuditLogDto>>;