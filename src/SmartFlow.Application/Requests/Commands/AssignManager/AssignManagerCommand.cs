using MediatR;

namespace SmartFlow.Application.Requests.Commands.AssignManager;

public sealed record AssignManagerCommand(
    Guid RequestId,
    Guid ManagerId,
    Guid PerformedById,
    uint Version) : IRequest<bool>;