using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> builder)
    {
        // Primary Key
        builder.HasKey(s => s.Id);

        // Properties
        builder.Property(s => s.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(s => s.TenantId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(s => s.ProductId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(s => s.Plan)
            .IsRequired();

        builder.Property(s => s.StartedAt)
            .IsRequired();

        builder.Property(s => s.ExpireAt);

        builder.Property(s => s.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        builder.Property(s => s.DeletedAt);

        // Indexes
        builder.HasIndex(s => s.TenantId);

        builder.HasIndex(s => s.ProductId);

        builder.HasIndex(s => s.Plan);

        builder.HasIndex(s => s.StartedAt);

        builder.HasIndex(s => s.ExpireAt);

        builder.HasIndex(s => s.DeletedAt);

        // Composite unique index to enforce one active subscription per tenant per product
        builder.HasIndex(s => new { s.TenantId, s.ProductId, s.DeletedAt })
            .IsUnique();

        // Composite index for active subscriptions by plan
        builder.HasIndex(s => new { s.Plan, s.DeletedAt });
    }
}
