
namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Permission>> GetPermissionsByRoleAsync(string roleId, CancellationToken ct = default);
    Task<IReadOnlyList<Permission>> GetPermissionsByUserAsync(string userId, string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetPermissionNamesByRoleAsync(string roleId, CancellationToken ct = default);
    Task AddAsync(Permission permission, CancellationToken ct = default);
    Task UpdateAsync(Permission permission, CancellationToken ct = default);
    Task DeleteAsync(Permission permission, CancellationToken ct = default);
    Task AddRolePermissionAsync(string roleId, string permissionId, CancellationToken ct = default);
    Task RemoveRolePermissionAsync(string roleId, string permissionId, CancellationToken ct = default);
}
