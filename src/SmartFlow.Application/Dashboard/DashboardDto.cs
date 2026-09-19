using SmartFlow.Application.Requests.Queries.GetRequests;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Application.Dashboard;

public sealed record DashboardDto(
    int TotalRequests,
    int DraftRequests,
    int SubmittedRequests,
    int UnderReviewRequests,
    int ApprovedRequests,
    int RejectedRequests,
    int UrgentRequests,
    int DueWithinSevenDays,
    int? ActiveUsers,
    IReadOnlyCollection<StatusCountDto> RequestsByStatus,
    IReadOnlyCollection<RequestListItemDto> RecentRequests);

public sealed record StatusCountDto(
    RequestStatus Status,
    int Count);