using MediatR;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Requests.Commands.UpdateRequest;

public sealed record UpdateRequestCommand(
    Guid RequestId,
    string Title,
    string Description,
    RequestPriority Priority,
    DateTime? DueDate,
    Guid CurrentUserId,
    uint Version) : IRequest<bool>;