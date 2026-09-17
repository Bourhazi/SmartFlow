using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Api.Contracts.Users;
using SmartFlow.Application.Common.Security;
using SmartFlow.Application.Users;
using SmartFlow.Application.Users.Commands.ChangeUserRole;
using SmartFlow.Application.Users.Commands.ChangeUserStatus;
using SmartFlow.Application.Users.Commands.CreateUser;
using SmartFlow.Application.Users.Queries.GetUserById;
using SmartFlow.Application.Users.Queries.GetUsers;

namespace SmartFlow.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = Roles.Administrateur)]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyCollection<UserDto>>(
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<UserDto>>> GetAll(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var users = await sender.Send(
            new GetUsersQuery(search),
            cancellationToken);

        return Ok(users);
    }

    [HttpGet("{userId:guid}")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(
            new GetUserByIdQuery(userId),
            cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType<UserDto>(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDto>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(
            new CreateUserCommand(
                request.FullName,
                request.Email,
                request.Password,
                request.Role),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { userId = user.Id },
            user);
    }

    [HttpPut("{userId:guid}/role")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> ChangeRole(
        Guid userId,
        ChangeUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(
            new ChangeUserRoleCommand(userId, request.Role),
            cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPut("{userId:guid}/status")]
    [ProducesResponseType<UserDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> ChangeStatus(
        Guid userId,
        ChangeUserStatusRequest request,
        CancellationToken cancellationToken)
    {
        var user = await sender.Send(
            new ChangeUserStatusCommand(userId, request.IsActive),
            cancellationToken);

        return user is null ? NotFound() : Ok(user);
    }
}