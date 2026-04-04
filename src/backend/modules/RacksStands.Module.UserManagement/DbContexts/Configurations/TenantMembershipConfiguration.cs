using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> builder)
    {
        // Primary Key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(m => m.TenantId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(m => m.UserId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(m => m.RoleId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(m => m.AssignedByUserId)
            .HasMaxLength(36);

        builder.Property(m => m.JoinedAt)
            .IsRequired();

        builder.Property(m => m.RevokedAt);

        builder.Property(m => m.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        builder.Property(m => m.DeletedAt);

        // Indexes
        builder.HasIndex(m => m.TenantId);

        builder.HasIndex(m => m.UserId);

        builder.HasIndex(m => m.RoleId);

        builder.HasIndex(m => m.AssignedByUserId);

        builder.HasIndex(m => m.JoinedAt);

        builder.HasIndex(m => m.RevokedAt);

        builder.HasIndex(m => m.DeletedAt);

        // Composite unique index to enforce one active membership per user per tenant
        builder.HasIndex(m => new { m.TenantId, m.UserId, m.DeletedAt })
            .IsUnique();

        // Composite index for active memberships by role
        builder.HasIndex(m => new { m.RoleId, m.DeletedAt });
    }
}
