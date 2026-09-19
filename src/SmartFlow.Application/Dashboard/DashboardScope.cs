namespace SmartFlow.Application.Dashboard;

public sealed record DashboardScope(
    Guid? CreatorId,
    Guid? ManagerId,
    bool IsAdministrator);