using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Commands.AddComment;

public sealed class AddCommentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork, 
    ICurrentUser currentUser)
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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}