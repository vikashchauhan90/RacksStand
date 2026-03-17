using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class M2MClientConfiguration : IEntityTypeConfiguration<M2MClient>
{
    public void Configure(EntityTypeBuilder<M2MClient> builder)
    {
        // Primary Key
        builder.HasKey(c => c.Id);

        // Properties
        builder.Property(c => c.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ClientId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ClientSecretHash)
            .IsRequired();

        builder.Property(c => c.Scopes)
            .IsRequired()
            .HasMaxLength(500); // assuming comma-separated scopes

        builder.Property(c => c.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.DeletedAt);

        // Indexes
        builder.HasIndex(c => c.ClientId)
            .IsUnique(); // enforce unique client IDs

        builder.HasIndex(c => c.Name);

        builder.HasIndex(c => c.DeletedAt);

        // Composite index for active clients by ClientId
        builder.HasIndex(c => new { c.ClientId, c.DeletedAt });

        // Composite index for scope-based queries
        builder.HasIndex(c => new { c.Scopes, c.DeletedAt });
    }
}
