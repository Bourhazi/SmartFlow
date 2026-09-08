using Microsoft.EntityFrameworkCore;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence;

public sealed class SmartFlowDbContext(
    DbContextOptions<SmartFlowDbContext> options)
    : DbContext(options)
{
    public DbSet<Request> Requests => Set<Request>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Attachment> Attachments => Set<Attachment>();

    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SmartFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}