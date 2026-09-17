using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(
    SmartFlowDbContext dbContext)
    : INotificationRepository
{
    public async Task AddAsync(
        Notification notification,
        CancellationToken cancellationToken)
    {
        await dbContext.Notifications.AddAsync(
            notification,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Notification>> GetForUserAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Notifications
            .AsNoTracking()
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<Notification?> GetForUserForUpdateAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return dbContext.Notifications.SingleOrDefaultAsync(
            notification =>
                notification.Id == notificationId &&
                notification.UserId == userId,
            cancellationToken);
    }
}