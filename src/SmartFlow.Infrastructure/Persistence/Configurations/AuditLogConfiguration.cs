using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Configurations;

public sealed class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.Id)
            .ValueGeneratedNever();

        builder.Property(log => log.Action)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(log => log.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(log => log.EntityId)
            .IsRequired();

        builder.Property(log => log.UserId)
            .IsRequired();

        builder.Property(log => log.OldValues)
            .HasColumnType("jsonb");

        builder.Property(log => log.NewValues)
            .HasColumnType("jsonb");

        builder.Property(log => log.IpAddress)
            .HasMaxLength(45);

        builder.Property(log => log.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(log => new
        {
            log.EntityType,
            log.EntityId,
            log.CreatedAtUtc
        });

        builder.HasIndex(log => new
        {
            log.UserId,
            log.CreatedAtUtc
        });
    }
}