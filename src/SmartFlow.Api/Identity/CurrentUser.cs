using System.Security.Claims;
using SmartFlow.Application.Common.Security;

namespace SmartFlow.Api.Identity;

public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User
                .FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var userId)
                ? userId
                : throw new UnauthorizedAccessException(
                    "The authenticated user identifier is missing.");
        }
    }

    public bool IsInRole(string role)
    {
        return httpContextAccessor.HttpContext?.User
            .IsInRole(role) ?? false;
    }
}