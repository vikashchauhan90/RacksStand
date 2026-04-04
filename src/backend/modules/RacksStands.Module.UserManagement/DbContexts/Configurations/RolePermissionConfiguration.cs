using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // Composite Primary Key
        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Properties
        builder.Property(rp => rp.RoleId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(rp => rp.PermissionId)
            .IsRequired()
            .HasMaxLength(36);

        // Indexes
        builder.HasIndex(rp => rp.RoleId);

        builder.HasIndex(rp => rp.PermissionId);

        // Composite index for quick lookups by both Role and Permission
        builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
            .IsUnique(); // ensures no duplicate mappings
    }
}
