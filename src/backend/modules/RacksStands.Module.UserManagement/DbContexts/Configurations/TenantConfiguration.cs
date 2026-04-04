using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.OwnerId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(t => t.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.DeletedAt);

        // Indexes
        builder.HasIndex(t => t.Name);

        builder.HasIndex(t => t.Slug)
            .IsUnique(); // enforce unique tenant slugs

        builder.HasIndex(t => t.OwnerId);

        builder.HasIndex(t => t.DeletedAt);

        // Composite index for active tenants
        builder.HasIndex(t => new { t.Slug, t.DeletedAt });
    }
}
