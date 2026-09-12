using MediatR;

namespace SmartFlow.Application.Requests.Commands.RemoveComment;

public sealed record RemoveCommentCommand(
    Guid RequestId,
    Guid CommentId,
    Guid CurrentUserId,
    uint Version) : IRequest<bool>;