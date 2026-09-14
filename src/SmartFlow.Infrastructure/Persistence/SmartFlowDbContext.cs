using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartFlow.Application.Common.Interfaces;
using SmartFlow.Domain.Entities;
using SmartFlow.Infrastructure.Identity;

namespace SmartFlow.Infrastructure.Persistence;

public sealed class SmartFlowDbContext(
    DbContextOptions<SmartFlowDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options),
      IUnitOfWork
{
    public DbSet<Request> Requests => Set<Request>();

    public DbSet<Comment> Comments => Set<Comment>();

    public DbSet<Attachment> Attachments => Set<Attachment>();

    public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SmartFlowDbContext).Assembly);

        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            builder.Property(user => user.FullName)
                .HasMaxLength(150)
                .IsRequired();
        });

        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
        CreateRole(
            new Guid("11111111-1111-1111-1111-111111111111"),
            AppRoles.Collaborateur),
        CreateRole(
            new Guid("22222222-2222-2222-2222-222222222222"),
            AppRoles.Manager),
        CreateRole(
            new Guid("33333333-3333-3333-3333-333333333333"),
            AppRoles.Administrateur));
    }

    private static IdentityRole<Guid> CreateRole(
    Guid id,
    string roleName)
    {
        return new IdentityRole<Guid>
        {
            Id = id,
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant(),
            ConcurrencyStamp = roleName
        };
    }
}