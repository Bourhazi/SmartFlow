    using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;
using SmartFlow.Domain.Entities;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Requests.Commands.ApproveRequest;

public sealed class ApproveRequestCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    INotificationRepository notificationRepository,
    IAuditLogger auditLogger)
    : IRequestHandler<ApproveRequestCommand, bool>
{
    public async Task<bool> Handle(
        ApproveRequestCommand command,
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

        request.Approve(
            currentUser.UserId,
            command.DecisionComment);
        
        await auditLogger.WriteAsync(
        "RequestApproved",
        "Request",
        request.Id,
        new { Status = "UnderReview" },
        new { Status = "Approved", command.DecisionComment },
        cancellationToken);

        
        await notificationRepository.AddAsync(
        Notification.Create(
            "Request approved",
            $"Your request '{request.Title}' was approved.",
            NotificationType.Approval,
            request.CreatorId,
            request.Id),
        cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}