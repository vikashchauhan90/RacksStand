using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts;

internal static class UserManagementDbSeeder
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Roles
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin",
                Description = "System administrator role",
                TenantId = "system",
                IsSystem = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User",
                Description = "Default user role",
                TenantId = "system",
                IsSystem = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow
            }
        );

        // Permissions
        modelBuilder.Entity<Permission>().HasData(
            new Permission
            {
                Id = Guid.NewGuid().ToString(),
                Name = "ManageUsers",
                Description = "Permission to manage users",
                Group = "UserManagement",
                TenantId = "system",
                IsSystem = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Permission
            {
                Id = Guid.NewGuid().ToString(),
                Name = "ManageRoles",
                Description = "Permission to manage roles",
                Group = "UserManagement",
                TenantId = "system",
                IsSystem = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow
            }
        );

        // Products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Basic Subscription",
                Description = "Free plan with limited features",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Premium Subscription",
                Description = "Paid plan with full features",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow
            }
        );

        // Tenants
        modelBuilder.Entity<Tenant>().HasData(
            new Tenant
            {
                Id = Guid.NewGuid().ToString(),
                Name = "System Tenant",
                Slug = "system",
                OwnerId = "system-owner",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow
            }
        );
    }
}
