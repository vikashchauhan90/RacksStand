using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Primary Key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(r => r.UserId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(r => r.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(r => r.IsRevoked)
            .IsRequired();

        builder.Property(r => r.ExpireAt)
            .IsRequired();

        builder.Property(r => r.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.Property(r => r.DeletedAt);

        // Indexes
        builder.HasIndex(r => r.UserId);

        builder.HasIndex(r => r.TokenHash)
            .IsUnique(); // enforce uniqueness of refresh tokens

        builder.HasIndex(r => r.ExpireAt);

        builder.HasIndex(r => r.IsRevoked);

        builder.HasIndex(r => r.DeletedAt);

        // Composite index for active tokens per user
        builder.HasIndex(r => new { r.UserId, r.IsRevoked, r.DeletedAt });
    }
}
