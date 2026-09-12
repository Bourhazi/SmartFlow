using MediatR;

namespace SmartFlow.Application.Requests.Commands.UpdateComment;

public sealed record UpdateCommentCommand(
    Guid RequestId,
    Guid CommentId,
    string Content,
    Guid CurrentUserId,
    uint Version) : IRequest<bool>;