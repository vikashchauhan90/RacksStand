using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class MagicLinkTokenConfiguration : IEntityTypeConfiguration<MagicLinkToken>
{
    public void Configure(EntityTypeBuilder<MagicLinkToken> builder)
    {
        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(t => t.UserId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(t => t.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.IsUsed)
            .IsRequired();

        builder.Property(t => t.IsRevoked)
            .IsRequired();

        builder.Property(t => t.ExpireAt)
            .IsRequired();

        builder.Property(t => t.UsedAt);

        builder.Property(t => t.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.DeletedAt);

        // Indexes
        builder.HasIndex(t => t.UserId);

        builder.HasIndex(t => t.Email);

        builder.HasIndex(t => t.TokenHash)
            .IsUnique(); // enforce uniqueness of token

        builder.HasIndex(t => t.ExpireAt);

        builder.HasIndex(t => t.IsUsed);

        builder.HasIndex(t => t.IsRevoked);

        builder.HasIndex(t => t.DeletedAt);

        // Composite index for active tokens per user
        builder.HasIndex(t => new { t.UserId, t.Email, t.IsUsed, t.IsRevoked, t.DeletedAt });
    }
}
