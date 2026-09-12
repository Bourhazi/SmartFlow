using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.UpdateComment;

public sealed class UpdateCommentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCommentCommand, bool>
{
    public async Task<bool> Handle(
        UpdateCommentCommand command,
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

        request.UpdateComment(
            command.CommentId,
            command.Content,
            command.CurrentUserId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}