using MediatR;

namespace SmartFlow.Application.Requests.Commands.RejectRequest;

public sealed record RejectRequestCommand(
    Guid RequestId,
    Guid ManagerId,
    string RejectionReason,
    uint Version) : IRequest<bool>;