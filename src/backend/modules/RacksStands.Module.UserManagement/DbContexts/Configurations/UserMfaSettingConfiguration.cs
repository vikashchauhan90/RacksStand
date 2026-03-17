
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class UserMfaSettingConfiguration : IEntityTypeConfiguration<UserMfaSetting>
{
    public void Configure(EntityTypeBuilder<UserMfaSetting> builder)
    {
        // Primary Key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(m => m.UserId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(m => m.RecoveryCodeHash)
            .IsRequired();

        builder.Property(m => m.TotpSecretEncrypted)
            .IsRequired();

        builder.Property(m => m.IsEnabled)
            .IsRequired();

        builder.Property(m => m.LastUsedAt);

        builder.Property(m => m.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        builder.Property(m => m.DeletedAt);

        // Indexes
        builder.HasIndex(m => m.UserId);

        builder.HasIndex(m => m.IsEnabled);

        builder.HasIndex(m => m.LastUsedAt);

        builder.HasIndex(m => m.DeletedAt);

        // Composite index for active MFA settings per user
        builder.HasIndex(m => new { m.UserId, m.IsEnabled, m.DeletedAt });
    }
}
