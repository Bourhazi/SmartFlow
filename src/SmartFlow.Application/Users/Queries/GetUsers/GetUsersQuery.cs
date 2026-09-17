using MediatR;

namespace SmartFlow.Application.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
    string? Search) : IRequest<IReadOnlyCollection<UserDto>>;