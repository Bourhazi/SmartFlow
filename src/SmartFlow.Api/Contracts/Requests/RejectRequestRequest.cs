namespace SmartFlow.Api.Contracts.Requests;

public sealed record RejectRequestRequest(
    Guid ManagerId,
    string RejectionReason,
    uint Version);