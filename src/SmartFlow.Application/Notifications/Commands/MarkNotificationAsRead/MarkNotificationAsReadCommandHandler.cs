using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Notifications.Commands.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler(
    INotificationRepository notificationRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    public async Task<bool> Handle(
        MarkNotificationAsReadCommand command,
        CancellationToken cancellationToken)
    {
        var notification =
            await notificationRepository.GetForUserForUpdateAsync(
                command.NotificationId,
                currentUser.UserId,
                cancellationToken);

        if (notification is null)
        {
            return false;
        }

        notification.MarkAsRead();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}