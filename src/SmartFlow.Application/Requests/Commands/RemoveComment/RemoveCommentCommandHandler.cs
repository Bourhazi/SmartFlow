using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.RemoveComment;

public sealed class RemoveCommentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
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
            command.CurrentUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}