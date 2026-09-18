using MediatR;

namespace SmartFlow.Application.Authentication.Commands.Logout;

public sealed record LogoutCommand(
    string RefreshToken) : IRequest;