using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;
using SmartFlow.Domain.Entities;
using SmartFlow.Domain.Enums;
namespace SmartFlow.Application.Requests.Commands.StartReview;

public sealed class StartReviewCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    INotificationRepository notificationRepository)
    : IRequestHandler<StartReviewCommand, bool>
{
    public async Task<bool> Handle(
        StartReviewCommand command,
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

        request.StartReview(currentUser.UserId);

        await notificationRepository.AddAsync(
        Notification.Create(
            "Request under review",
            $"Your request '{request.Title}' is now under review.",
            NotificationType.Assignment,
            request.CreatorId,
            request.Id),
        cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}