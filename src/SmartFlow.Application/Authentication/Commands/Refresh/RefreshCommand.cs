using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Refresh;

public sealed record RefreshCommand(
    string RefreshToken) : IRequest<AuthResult>;