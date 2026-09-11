using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Configurations;

public sealed class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests");

        builder.HasKey(request => request.Id);

        builder.Property(request => request.Id)
            .ValueGeneratedNever();

        builder.Property(request => request.Title)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(request => request.Description)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(request => request.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(request => request.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(request => request.CreatorId)
            .IsRequired();

        builder.Property(request => request.AssignedManagerId);

        builder.Property(request => request.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(request => request.DueDate)
            .HasColumnType("timestamp with time zone");

        builder.Property(request => request.SubmittedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(request => request.DecisionAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.Property(request => request.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(request => request.UpdatedAtUtc)
            .HasColumnType("timestamp with time zone");

        // Propriété invisible dans Domain, mappée sur la colonne système PostgreSQL xmin.
        // EF Core lèvera une DbUpdateConcurrencyException si deux mises à jour
        // concurrentes tentent de modifier la même demande.
        builder.Property(request => request.Version)
        .IsRowVersion();

        builder.HasIndex(request => request.CreatorId);

        builder.HasIndex(request => request.AssignedManagerId);

        builder.HasIndex(request => request.Status);

        builder.HasMany(request => request.Attachments)
            .WithOne()
            .HasForeignKey(attachment => attachment.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(request => request.Attachments)
            .HasField("_attachments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(request => request.Comments)
            .WithOne()
            .HasForeignKey(comment => comment.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(request => request.Comments)
            .HasField("_comments")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(request => request.ApprovalHistories)
            .WithOne()
            .HasForeignKey(history => history.RequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(request => request.ApprovalHistories)
            .HasField("_approvalHistory")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}