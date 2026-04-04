using Microsoft.EntityFrameworkCore;
using RacksStands.Module.UserManagement.DbContexts.Entities;

namespace RacksStands.Module.UserManagement.DbContexts;

internal class UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<UserMfaSetting> UserMfaSettings { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;
    public DbSet<M2MClient> M2MClients { get; set; } = default!;
    public DbSet<MagicLinkToken> MagicLinkTokens { get; set; } = default!;
    public DbSet<MfaChallenge> MfaChallenges { get; set; } = default!;
    public DbSet<Permission> Permissions { get; set; } = default!;
    public DbSet<Product> Products { get; set; } = default!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = default!;
    public DbSet<RolePermission> RolePermissions { get; set; } = default!;
    public DbSet<Tenant> Tenants { get; set; } = default!;
    public DbSet<TenantInvitation> TenantInvitations { get; set; } = default!;
    public DbSet<TenantMembership> TenantMemberships { get; set; } = default!;
    public DbSet<TenantSubscription> TenantSubscriptions { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserManagementDbContext).Assembly);
        UserManagementDbSeeder.Seed(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
}
