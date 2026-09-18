using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;
using SmartFlow.Domain.Enums;
using SmartFlow.Infrastructure.Persistence;

namespace SmartFlow.Infrastructure.Notifications;

public sealed class DeadlineNotificationService(
    SmartFlowDbContext dbContext,
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork)
    : IDeadlineNotificationService
{
    private static readonly int[] AlertDays =
    [
        7,
        3,
        1
    ];

    public async Task ProcessAsync(
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        foreach (var daysRemaining in AlertDays)
        {
            var dayStart = utcNow.Date.AddDays(daysRemaining);
            var dayEnd = dayStart.AddDays(1);

            var requests = await dbContext.Requests
                .Where(request =>
                    request.DueDate.HasValue &&
                    request.DueDate.Value >= dayStart &&
                    request.DueDate.Value < dayEnd &&
                    request.Status != RequestStatus.Approved &&
                    request.Status != RequestStatus.Rejected)
                .ToListAsync(cancellationToken);

            foreach (var request in requests)
            {
                await CreateNotificationsForRequestAsync(
                    request,
                    daysRemaining,
                    cancellationToken);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task CreateNotificationsForRequestAsync(
        Request request,
        int daysRemaining,
        CancellationToken cancellationToken)
    {
        var title = $"Deadline in {daysRemaining} day(s)";

        var recipients = new[]
        {
            request.CreatorId,
            request.AssignedManagerId
        }
        .Where(userId => userId.HasValue)
        .Select(userId => userId!.Value)
        .Distinct();

        foreach (var recipientId in recipients)
        {
            var alreadyExists = await notificationRepository.ExistsAsync(
                recipientId,
                request.Id,
                NotificationType.Deadline,
                title,
                cancellationToken);

            if (alreadyExists)
            {
                continue;
            }

            await notificationRepository.AddAsync(
                Notification.Create(
                    title,
                    $"The request '{request.Title}' has a deadline in {daysRemaining} day(s).",
                    NotificationType.Deadline,
                    recipientId,
                    request.Id),
                cancellationToken);
        }
    }
}