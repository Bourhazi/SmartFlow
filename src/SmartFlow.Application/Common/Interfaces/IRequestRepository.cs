using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Common.Interfaces;

public interface IRequestRepository
{
    Task AddAsync(Request request, CancellationToken cancellationToken);

    Task<Request?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken);
    Task<Request?> GetForUpdateAsync(
    Guid requestId,
    uint expectedVersion,
    CancellationToken cancellationToken);
} 