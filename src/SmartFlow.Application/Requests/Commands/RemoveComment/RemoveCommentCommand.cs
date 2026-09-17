using MediatR;

namespace SmartFlow.Application.Requests.Commands.RemoveComment;

public sealed record RemoveCommentCommand(
    Guid RequestId,
    Guid CommentId,
    uint Version) : IRequest<bool>;