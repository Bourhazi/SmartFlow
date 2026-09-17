using MediatR;

namespace SmartFlow.Application.Users.Commands.ChangeUserRole;

public sealed record ChangeUserRoleCommand(
    Guid UserId,
    string Role) : IRequest<UserDto?>;