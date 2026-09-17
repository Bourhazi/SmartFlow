using MediatR;

namespace SmartFlow.Application.Users.Queries.GetUserById;

public sealed record GetUserByIdQuery(
    Guid UserId) : IRequest<UserDto?>;