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
}