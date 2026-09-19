using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Dashboard;
using SmartFlow.Application.Requests.Queries.GetRequests;
using SmartFlow.Domain.Entities;
using SmartFlow.Domain.Enums;

namespace SmartFlow.Infrastructure.Persistence.Queries;

public sealed class DashboardReadService(
    SmartFlowDbContext dbContext)
    : IDashboardReadService
{
    public async Task<DashboardDto> GetAsync(
        DashboardScope scope,
        CancellationToken cancellationToken)
    {
        var query = ApplyScope(scope);

        var totalRequests = await query.CountAsync(cancellationToken);

        var draftRequests = await query.CountAsync(
            request => request.Status == RequestStatus.Draft,
            cancellationToken);

        var submittedRequests = await query.CountAsync(
            request => request.Status == RequestStatus.Submitted,
            cancellationToken);

        var underReviewRequests = await query.CountAsync(
            request => request.Status == RequestStatus.UnderReview,
            cancellationToken);

        var approvedRequests = await query.CountAsync(
            request => request.Status == RequestStatus.Approved,
            cancellationToken);

        var rejectedRequests = await query.CountAsync(
            request => request.Status == RequestStatus.Rejected,
            cancellationToken);

        var urgentRequests = await query.CountAsync(
            request => request.Priority == RequestPriority.Urgent,
            cancellationToken);

        var now = DateTime.UtcNow;
        var sevenDaysLater = now.AddDays(7);

        var dueWithinSevenDays = await query.CountAsync(
            request =>
                request.DueDate.HasValue &&
                request.DueDate >= now &&
                request.DueDate <= sevenDaysLater &&
                request.Status != RequestStatus.Approved &&
                request.Status != RequestStatus.Rejected,
            cancellationToken);

        int? activeUsers = null;

        if (scope.IsAdministrator)
        {
            activeUsers = await dbContext.Users.CountAsync(
                user => user.IsActive,
                cancellationToken);
        }

        var statusCounts = await query
            .GroupBy(request => request.Status)
            .Select(group => new
            {
                Status = group.Key,
                Count = group.Count()
            })
            .OrderBy(item => item.Status)
            .ToListAsync(cancellationToken);

        var requestsByStatus = statusCounts
            .Select(item => new StatusCountDto(
                item.Status,
                item.Count))
            .ToArray();

        var recentRequests = await GetRecentRequestsAsync(
            scope,
            cancellationToken);

        return new DashboardDto(
            totalRequests,
            draftRequests,
            submittedRequests,
            underReviewRequests,
            approvedRequests,
            rejectedRequests,
            urgentRequests,
            dueWithinSevenDays,
            activeUsers,
            requestsByStatus,
            recentRequests);
    }

    public async Task<IReadOnlyCollection<RequestListItemDto>>
        GetRecentRequestsAsync(
            DashboardScope scope,
            CancellationToken cancellationToken)
    {
        return await ApplyScope(scope)
            .OrderByDescending(request => request.CreatedAtUtc)
            .ThenByDescending(request => request.Id)
            .Take(10)
            .Select(request => new RequestListItemDto(
                request.Id,
                request.Title,
                request.Status,
                request.Priority,
                request.DueDate,
                request.CreatorId,
                request.AssignedManagerId,
                request.CreatedAtUtc,
                request.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Request> ApplyScope(DashboardScope scope)
    {
        IQueryable<Request> query = dbContext.Requests
            .AsNoTracking();

        if (scope.IsAdministrator)
        {
            return query;
        }

        if (scope.ManagerId.HasValue)
        {
            return query.Where(request =>
                request.AssignedManagerId == scope.ManagerId.Value);
        }

        return query.Where(request =>
            request.CreatorId == scope.CreatorId!.Value);
    }
}