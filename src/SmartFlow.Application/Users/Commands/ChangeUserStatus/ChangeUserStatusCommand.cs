using MediatR;

namespace SmartFlow.Application.Users.Commands.ChangeUserStatus;

public sealed record ChangeUserStatusCommand(
    Guid UserId,
    bool IsActive) : IRequest<UserDto?>;