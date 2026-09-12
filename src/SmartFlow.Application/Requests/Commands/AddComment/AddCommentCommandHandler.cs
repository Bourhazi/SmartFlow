using MediatR;
using SmartFlow.Application.Common.Interfaces;

namespace SmartFlow.Application.Requests.Commands.AddComment;

public sealed class AddCommentCommandHandler(
    IRequestRepository requestRepository,
    IUnitOfWork unitOfWork)
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
            command.AuthorId);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}