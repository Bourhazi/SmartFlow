using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Repositories;

public sealed class RequestRepository(
    SmartFlowDbContext dbContext)
    : IRequestRepository
{
    public async Task AddAsync(
        Request request,
        CancellationToken cancellationToken)
    {
        await dbContext.Requests.AddAsync(request, cancellationToken);
    }

    public Task<Request?> GetByIdAsync(
        Guid requestId,
        CancellationToken cancellationToken)
    {
        return dbContext.Requests
            .Include(request => request.Attachments)
            .Include(request => request.Comments)
            .Include(request => request.ApprovalHistories)
            .SingleOrDefaultAsync(
                request => request.Id == requestId,
                cancellationToken);
    }

    public async Task<Request?> GetForUpdateAsync(
    Guid requestId,
    uint expectedVersion,
    CancellationToken cancellationToken)
    {
        var request = await dbContext.Requests
            .SingleOrDefaultAsync(
                request => request.Id == requestId,
                cancellationToken);

        if (request is not null)
        {
            dbContext.Entry(request)
                .Property(entity => entity.Version)
                .OriginalValue = expectedVersion;
        }

        return request;
    }
}