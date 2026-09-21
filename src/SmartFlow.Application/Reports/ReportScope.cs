namespace SmartFlow.Application.Reports;

public sealed record ReportScope(
    Guid? CreatorId,
    Guid? ManagerId,
    bool IsAdministrator);