using MediatR;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Application.Requests.Commands.RemoveComment;

public sealed class RemoveCommentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IRequestHandler<RemoveCommentCommand, bool>
{
    public async Task<bool> Handle(
        RemoveCommentCommand command,
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

        request.RemoveComment(
            command.CommentId,
            currentUser.UserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}