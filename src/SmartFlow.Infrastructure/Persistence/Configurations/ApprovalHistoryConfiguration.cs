using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Configurations;

public sealed class ApprovalHistoryConfiguration
    : IEntityTypeConfiguration<ApprovalHistory>
{
    public void Configure(EntityTypeBuilder<ApprovalHistory> builder)
    {
        builder.ToTable("ApprovalHistories");

        builder.HasKey(history => history.Id);

        builder.Property(history => history.Id)
            .ValueGeneratedNever();

        builder.Property(history => history.RequestId)
            .IsRequired();

        builder.Property(history => history.OldStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(history => history.NewStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(history => history.PerformedById)
            .IsRequired();

        builder.Property(history => history.Comment)
            .HasMaxLength(1000);

        builder.Property(history => history.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(history => history.UpdatedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(history => history.RequestId);

        builder.HasIndex(history => history.PerformedById);
    }
}