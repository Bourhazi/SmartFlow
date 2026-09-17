using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResult>;