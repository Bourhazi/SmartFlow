using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;
using SmartFlow.Domain.Entities;
using SmartFlow.Domain.Enums;
namespace SmartFlow.Application.Requests.Commands.AddComment;

public sealed class AddCommentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork, 
    ICurrentUser currentUser,
    INotificationRepository notificationRepository)
    : IRequestHandler<AddCommentCommand, Guid?>
{
    public async Task<Guid?> Handle(
        AddCommentCommand command,
        CancellationToken cancellationToken)
    {
        var request = await requestRepository.GetForUpdateAsync(
            command.RequestId,
            command.Version,
            cancellationToken);

        if (request is null)
        {
            return null;
        }

        var comment = request.AddComment(
            command.Content,
            currentUser.UserId);

        var recipients = new[]
    {
        request.CreatorId,
        request.AssignedManagerId
    }
    .Where(userId =>
        userId.HasValue &&
        userId.Value != currentUser.UserId)
    .Select(userId => userId!.Value)
    .Distinct();

    foreach (var recipientId in recipients)
    {
        await notificationRepository.AddAsync(
            Notification.Create(
                "New comment",
                $"A new comment was added to '{request.Title}'.",
                NotificationType.Comment,
                recipientId,
                request.Id),
            cancellationToken);
    }
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}