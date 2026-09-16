using MediatR;

namespace SmartFlow.Application.Requests.Commands.SubmitRequest;

public sealed record SubmitRequestCommand(
    Guid RequestId,
    uint Version) : IRequest<bool>;