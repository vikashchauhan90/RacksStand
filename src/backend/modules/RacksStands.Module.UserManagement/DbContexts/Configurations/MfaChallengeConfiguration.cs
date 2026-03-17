using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class MfaChallengeConfiguration : IEntityTypeConfiguration<MfaChallenge>
{
    public void Configure(EntityTypeBuilder<MfaChallenge> builder)
    {
        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(c => c.UserId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(c => c.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.IsUsed)
            .IsRequired();

        builder.Property(c => c.ExpireAt)
            .IsRequired();

        builder.Property(c => c.UsedAt);

        builder.Property(c => c.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Indexes
        builder.HasIndex(c => c.UserId);

        builder.HasIndex(c => c.TokenHash)
            .IsUnique(); // enforce uniqueness of challenge tokens

        builder.HasIndex(c => c.ExpireAt);

        builder.HasIndex(c => c.IsUsed);

        builder.HasIndex(c => c.DeletedAt);

        // Composite index for active challenges per user
        builder.HasIndex(c => new { c.UserId, c.IsUsed, c.DeletedAt });
    }
}
