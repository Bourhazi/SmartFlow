using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Infrastructure.Notifications;

public sealed class DeadlineNotificationWorker(
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DeadlineNotificationWorker> logger)
    : BackgroundService
{
    private static readonly TimeSpan ExecutionInterval =
        TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDeadlinesAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception,
                    "An error occurred while processing deadline notifications.");
            }

            await Task.Delay(
                ExecutionInterval,
                stoppingToken);
        }
    }

    private async Task ProcessDeadlinesAsync(
        CancellationToken cancellationToken)
    {
        await using var scope =
            serviceScopeFactory.CreateAsyncScope();

        var service = scope.ServiceProvider
            .GetRequiredService<IDeadlineNotificationService>();

        await service.ProcessAsync(
            DateTime.UtcNow,
            cancellationToken);
    }
}