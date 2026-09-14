using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartFlow.Api.Contracts.Authentication;
using SmartFlow.Application.Authentication;
using SmartFlow.Application.Authentication.Commands.Login;
using SmartFlow.Application.Authentication.Commands.Register;

namespace SmartFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType<AuthResult>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResult>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RegisterCommand(
                request.FullName,
                request.Email,
                request.Password),
            cancellationToken);

        return Created(string.Empty, result);
    }

    [HttpPost("login")]
    [ProducesResponseType<AuthResult>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResult>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        return Ok(result);
    }
}