using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(notification => notification.Id);

        builder.Property(notification => notification.Id)
            .ValueGeneratedNever();

        builder.Property(notification => notification.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(notification => notification.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(notification => notification.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(notification => notification.IsRead)
            .IsRequired();

        builder.Property(notification => notification.UserId)
            .IsRequired();

        builder.Property(notification => notification.RequestId);

        builder.Property(notification => notification.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(notification => notification.UpdatedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(notification => notification.UserId);

        builder.HasIndex(notification => new
        {
            notification.UserId,
            notification.IsRead,
            notification.CreatedAtUtc
        });
    }
}