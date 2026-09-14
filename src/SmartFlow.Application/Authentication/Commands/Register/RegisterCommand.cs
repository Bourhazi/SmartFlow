using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Register;

public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password) : IRequest<AuthResult>;