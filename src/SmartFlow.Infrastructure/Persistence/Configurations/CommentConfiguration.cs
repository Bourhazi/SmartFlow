using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartFlow.Domain.Entities;

namespace SmartFlow.Infrastructure.Persistence.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments");

        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Id)
            .ValueGeneratedNever();

        builder.Property(comment => comment.RequestId)
            .IsRequired();

        builder.Property(comment => comment.AuthorId)
            .IsRequired();

        builder.Property(comment => comment.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(comment => comment.CreatedAtUtc)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(comment => comment.UpdatedAtUtc)
            .HasColumnType("timestamp with time zone");

        builder.HasIndex(comment => comment.RequestId);

        builder.HasIndex(comment => comment.AuthorId);
    }
}