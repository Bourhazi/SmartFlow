using MediatR;

namespace SmartFlow.Application.Requests.Commands.AddComment;

public sealed record AddCommentCommand(
    Guid RequestId,
    string Content,
    uint Version) : IRequest<Guid?>;