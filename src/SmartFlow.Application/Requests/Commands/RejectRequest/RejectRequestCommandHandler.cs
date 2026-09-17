using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;
using SmartFlow.Domain.Entities;
using SmartFlow.Domain.Enums;
namespace SmartFlow.Application.Requests.Commands.RejectRequest;

public sealed class RejectRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    INotificationRepository notificationRepository)
    : IRequestHandler<RejectRequestCommand, bool>
{
    public async Task<bool> Handle(
        RejectRequestCommand command,
        CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetForUpdateAsync(
            command.RequestId,
            command.Version,
            cancellationToken);

        if (request is null)
        {
            return false;
        }

        request.Reject(
            currentUser.UserId,
            command.RejectionReason);
        await notificationRepository.AddAsync(
        Notification.Create(
            "Request rejected",
            $"Your request '{request.Title}' was rejected.",
            NotificationType.Rejection,
            request.CreatorId,
            request.Id),
        cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}