using SmartFlow.Domain.Entities;

namespace SmartFlow.Application.Common.Interfaces;

public interface INotificationRepository
{
    Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Notification>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<Notification?> GetForUserForUpdateAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken);
}