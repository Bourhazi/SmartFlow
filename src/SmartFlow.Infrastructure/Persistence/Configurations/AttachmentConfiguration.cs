using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Configurations;

public sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");

        builder.HasKey(attachment => attachment.Id);

        builder.Property(attachment => attachment.Id)
            .ValueGeneratedNever();

        builder.Property(attachment => attachment.RequestId)
            .IsRequired();

        builder.Property(attachment => attachment.UploadedById)
            .IsRequired();

        builder.Property(attachment => attachment.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(attachment => attachment.StorageFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(attachment => attachment.ContentType)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(attachment => attachment.Size)
            .IsRequired();

        builder.Property(attachment => attachment.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(attachment => attachment.UpdatedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(attachment => attachment.RequestId);

        builder.HasIndex(attachment => attachment.UploadedById);
    }
}