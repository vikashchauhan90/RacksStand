namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface ITenantMembershipRepository
{
    Task<TenantMembership?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<TenantMembership?> GetByUserAndTenantAsync(string userId, string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<TenantMembership>> GetMembershipsByUserAsync(string userId, CancellationToken ct = default);
    Task<IReadOnlyList<TenantMembership>> GetMembershipsByTenantAsync(string tenantId, CancellationToken ct = default);
    Task AddAsync(TenantMembership membership, CancellationToken ct = default);
    Task UpdateAsync(TenantMembership membership, CancellationToken ct = default);
    Task RevokeAsync(TenantMembership membership, CancellationToken ct = default);
}
