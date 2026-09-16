using MediatR;

namespace SmartFlow.Application.Requests.Commands.RejectRequest;

public sealed record RejectRequestCommand(
    Guid RequestId,
    string RejectionReason,
    uint Version) : IRequest<bool>;