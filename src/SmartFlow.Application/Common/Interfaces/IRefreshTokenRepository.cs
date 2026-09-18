using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Common.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken);

    Task<RefreshToken?> GetForUpdateAsync(
        string tokenHash,
        CancellationToken cancellationToken);

    Task RevokeAllForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);
}