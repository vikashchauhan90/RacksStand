using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Primary Key
        builder.HasKey(u => u.Id);

        // Properties
        builder.Property(u => u.Id)
            .IsRequired()
            .HasMaxLength(36); // assuming GUID-like string

        builder.Property(u => u.Name)
            .HasMaxLength(100);

        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.ConcurrencyStamp)
            .IsConcurrencyToken()
            .HasMaxLength(40);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.DeletedAt);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.HasIndex(u => u.CreatedAt);

        builder.HasIndex(u => u.DeletedAt);

        // Optional: composite index for active users (Email + DeletedAt)
        builder.HasIndex(u => new { u.Email, u.DeletedAt });
    }
}
