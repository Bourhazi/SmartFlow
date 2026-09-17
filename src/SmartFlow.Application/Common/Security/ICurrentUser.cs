namespace SmartFlow.Application.Common.Security;

public interface ICurrentUser
{
    Guid UserId { get; }

    bool IsInRole(string role);
}