using MediatR;

namespace SmartFlow.Application.Requests.Commands.UpdateComment;

public sealed record UpdateCommentCommand(
    Guid RequestId,
    Guid CommentId,
    string Content,
    uint Version) : IRequest<bool>;