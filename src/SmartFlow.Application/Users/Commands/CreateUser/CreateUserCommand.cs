using MediatR;

namespace SmartFlow.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string FullName,
    string Email,
    string Password,
    string Role) : IRequest<UserDto>;