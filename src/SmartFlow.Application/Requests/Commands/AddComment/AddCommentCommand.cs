using MediatR;

namespace SmartFlow.Application.Requests.Commands.AddComment;

public sealed record AddCommentCommand(
    Guid RequestId,
    string Content,
    Guid AuthorId,
    uint Version) : IRequest<Guid?>;