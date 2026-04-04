using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(250);

        builder.Property(p => p.Group)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.TenantId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(p => p.IsSystem)
            .IsRequired();

        builder.Property(p => p.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.DeletedAt);

        // Indexes
        builder.HasIndex(p => p.Name);

        builder.HasIndex(p => p.Group);

        builder.HasIndex(p => p.TenantId);

        builder.HasIndex(p => p.IsSystem);

        builder.HasIndex(p => p.DeletedAt);

        // Composite unique index to enforce unique permission names per tenant
        builder.HasIndex(p => new { p.TenantId, p.Name })
            .IsUnique();

        // Composite index for active system permissions
        builder.HasIndex(p => new { p.IsSystem, p.DeletedAt });
    }
}
