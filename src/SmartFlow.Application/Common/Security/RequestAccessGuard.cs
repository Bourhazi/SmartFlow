using SmartFlow.Application.Common.Exceptions;

namespace SmartFlow.Application.Common.Security;

public sealed class RequestAccessGuard(
    ICurrentUser currentUser)
{
    public void EnsureCanRead(
        Guid creatorId,
        Guid? assignedManagerId)
    {
        if (currentUser.IsInRole(Roles.Administrateur))
        {
            return;
        }

        if (currentUser.IsInRole(Roles.Manager))
        {
            if (assignedManagerId == currentUser.UserId)
            {
                return;
            }

            throw new ForbiddenAccessException(
                "A manager can only access requests assigned to them.");
        }

        if (creatorId == currentUser.UserId)
        {
            return;
        }

        throw new ForbiddenAccessException(
            "You can only access requests that you created.");
    }
}