using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Application.Common.Models;
using SmartFlow.Application.Requests.Queries.GetRequests;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Queries;

public sealed class RequestReadService(
    SmartFlowDbContext dbContext)
    : IRequestReadService
{
    public async Task<PagedResult<RequestListItemDto>> GetPagedAsync(
        GetRequestsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<Request> query = dbContext.Requests
            .AsNoTracking();

        if (request.Status.HasValue)
        {
            query = query.Where(item =>
                item.Status == request.Status.Value);
        }

        if (request.Priority.HasValue)
        {
            query = query.Where(item =>
                item.Priority == request.Priority.Value);
        }

        if (request.CreatorId.HasValue)
        {
            query = query.Where(item =>
                item.CreatorId == request.CreatorId.Value);
        }

        if (request.ManagerId.HasValue)
        {
            query = query.Where(item =>
                item.AssignedManagerId == request.ManagerId.Value);
        }

        var search = request.Search?.Trim();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(item =>
                EF.Functions.ILike(
                    item.Title,
                    $"%{search}%") ||
                EF.Functions.ILike(
                    item.Description,
                    $"%{search}%"));
        }

        query = ApplySorting(query, request);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(item => new RequestListItemDto(
                item.Id,
                item.Title,
                item.Status,
                item.Priority,
                item.DueDate,
                item.CreatorId,
                item.AssignedManagerId,
                item.CreatedAtUtc,
                item.UpdatedAtUtc))
            .ToListAsync(cancellationToken);

        var totalPages = (int)Math.Ceiling(
            totalCount / (double)request.PageSize);

        return new PagedResult<RequestListItemDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount,
            totalPages);
    }

    private static IQueryable<Request> ApplySorting(
        IQueryable<Request> query,
        GetRequestsQuery request)
    {
        return request.SortBy switch
        {
            RequestSortBy.Title when
                request.SortDirection == SortDirection.Asc =>
                query.OrderBy(item => item.Title)
                    .ThenBy(item => item.Id),

            RequestSortBy.Title =>
                query.OrderByDescending(item => item.Title)
                    .ThenBy(item => item.Id),

            RequestSortBy.DueDate when
                request.SortDirection == SortDirection.Asc =>
                query.OrderBy(item => item.DueDate)
                    .ThenBy(item => item.Id),

            RequestSortBy.DueDate =>
                query.OrderByDescending(item => item.DueDate)
                    .ThenBy(item => item.Id),

            RequestSortBy.Priority when
                request.SortDirection == SortDirection.Asc =>
                query.OrderBy(item => item.Priority)
                    .ThenBy(item => item.Id),

            RequestSortBy.Priority =>
                query.OrderByDescending(item => item.Priority)
                    .ThenBy(item => item.Id),

            RequestSortBy.Status when
                request.SortDirection == SortDirection.Asc =>
                query.OrderBy(item => item.Status)
                    .ThenBy(item => item.Id),

            RequestSortBy.Status =>
                query.OrderByDescending(item => item.Status)
                    .ThenBy(item => item.Id),

            RequestSortBy.CreatedAt when
                request.SortDirection == SortDirection.Asc =>
                query.OrderBy(item => item.CreatedAtUtc)
                    .ThenBy(item => item.Id),

            _ => query.OrderByDescending(item => item.CreatedAtUtc)
                .ThenBy(item => item.Id)
        };
    }
}