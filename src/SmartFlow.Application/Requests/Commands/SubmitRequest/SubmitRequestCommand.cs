using MediatR;

namespace SmartFlow.Application.Requests.Commands.SubmitRequest;

public sealed record SubmitRequestCommand(
    Guid RequestId,
    Guid CurrentUserId,
    uint Version) : IRequest<bool>;