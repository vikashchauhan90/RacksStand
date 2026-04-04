using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Primary Key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Description)
            .HasMaxLength(250);

        builder.Property(r => r.TenantId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(r => r.IsSystem)
            .IsRequired();

        builder.Property(r => r.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt);

        builder.Property(r => r.DeletedAt);

        // Indexes
        builder.HasIndex(r => r.Name);

        builder.HasIndex(r => r.TenantId);

        builder.HasIndex(r => r.IsSystem);

        builder.HasIndex(r => r.DeletedAt);

        // Composite unique index to enforce unique role names per tenant
        builder.HasIndex(r => new { r.TenantId, r.Name })
            .IsUnique();

        // Composite index for filtering active system roles
        builder.HasIndex(r => new { r.IsSystem, r.DeletedAt });
    }
}
