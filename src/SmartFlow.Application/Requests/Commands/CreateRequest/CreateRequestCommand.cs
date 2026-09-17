using MediatR;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Requests.Commands.CreateRequest;

public sealed record CreateRequestCommand(
    string Title,
    string Description,
    RequestPriority Priority,
    DateTime? DueDate) : IRequest<Guid>;    