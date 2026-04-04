using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class TenantInvitationConfiguration : IEntityTypeConfiguration<TenantInvitation>
{
    public void Configure(EntityTypeBuilder<TenantInvitation> builder)
    {
        // Primary Key
        builder.HasKey(i => i.Id);

        // Properties
        builder.Property(i => i.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(i => i.TenantId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(i => i.TenantSubscriptionId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(i => i.RoleId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.TokenHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(i => i.InvitedByUserId)
            .HasMaxLength(36);

        builder.Property(i => i.Status)
            .IsRequired();

        builder.Property(i => i.ExpireAt);

        builder.Property(i => i.RevokedAt);

        builder.Property(i => i.AcceptedByUserId)
            .HasMaxLength(36);

        builder.Property(i => i.RespondedAt);

        builder.Property(i => i.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        builder.Property(i => i.DeletedAt);

        // Indexes
        builder.HasIndex(i => i.TenantId);

        builder.HasIndex(i => i.TenantSubscriptionId);

        builder.HasIndex(i => i.RoleId);

        builder.HasIndex(i => i.Email);

        builder.HasIndex(i => i.TokenHash)
            .IsUnique(); // enforce uniqueness of invitation tokens

        builder.HasIndex(i => i.Status);

        builder.HasIndex(i => i.ExpireAt);

        builder.HasIndex(i => i.RevokedAt);

        builder.HasIndex(i => i.AcceptedByUserId);

        builder.HasIndex(i => i.RespondedAt);

        builder.HasIndex(i => i.DeletedAt);

        // Composite index for active invitations per tenant
        builder.HasIndex(i => new { i.TenantId, i.Email, i.Status, i.DeletedAt });
    }
}
