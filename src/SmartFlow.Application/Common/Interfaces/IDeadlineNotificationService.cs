namespace SmartFlow.Application.Common.Interfaces;

public interface IDeadlineNotificationService
{
    Task ProcessAsync(
        DateTime utcNow,
        CancellationToken cancellationToken);
}