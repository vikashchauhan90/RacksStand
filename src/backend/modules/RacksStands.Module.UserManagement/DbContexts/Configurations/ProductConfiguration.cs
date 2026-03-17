using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Primary Key
        builder.HasKey(p => p.Id);

        // Properties
        builder.Property(p => p.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Description)
            .HasMaxLength(500);

        builder.Property(p => p.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        builder.Property(p => p.DeletedAt);

        // Indexes
        builder.HasIndex(p => p.Name);

        builder.HasIndex(p => p.DeletedAt);

        // Composite unique index to enforce unique product names
        builder.HasIndex(p => p.Name)
            .IsUnique();

        // Composite index for active products
        builder.HasIndex(p => new { p.Name, p.DeletedAt });
    }
}
