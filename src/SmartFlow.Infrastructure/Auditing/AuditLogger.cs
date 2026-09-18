using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Auditing;

public sealed class AuditLogger(
    IAuditLogRepository auditLogRepository,
    ICurrentUser currentUser,
    IHttpContextAccessor httpContextAccessor)
    : IAuditLogger
{
    public Task WriteAsync(
        string action,
        string entityType,
        Guid entityId,
        object? oldValues,
        object? newValues,
        CancellationToken cancellationToken)
    {
        var ipAddress = httpContextAccessor.HttpContext?
            .Connection
            .RemoteIpAddress?
            .ToString();

        var auditLog = AuditLog.Create(
            action,
            entityType,
            entityId,
            currentUser.UserId,
            Serialize(oldValues),
            Serialize(newValues),
            ipAddress);

        return auditLogRepository.AddAsync(
            auditLog,
            cancellationToken);
    }

    private static string? Serialize(object? values)
    {
        return values is null
            ? null
            : JsonSerializer.Serialize(values);
    }
}