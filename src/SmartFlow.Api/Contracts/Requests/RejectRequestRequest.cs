namespace SmartFlow.Api.Contracts.Requests;

public sealed record RejectRequestRequest(
    string RejectionReason,
    uint Version);