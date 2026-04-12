namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IRoleRepository
{
    Task<Role?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Role?> GetByNameAndTenantAsync(string name, string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<Role>> GetRolesByTenantAsync(string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetRoleNamesByUserAsync(string userId, string tenantId, CancellationToken ct = default);
    Task AddAsync(Role role, CancellationToken ct = default);
    Task UpdateAsync(Role role, CancellationToken ct = default);
    Task DeleteAsync(Role role, CancellationToken ct = default);
}
