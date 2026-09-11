using MediatR;

namespace SmartFlow.Application.Requests.Commands.ApproveRequest;

public sealed record ApproveRequestCommand(
    Guid RequestId,
    Guid ManagerId,
    string? DecisionComment,
    uint Version) : IRequest<bool>;