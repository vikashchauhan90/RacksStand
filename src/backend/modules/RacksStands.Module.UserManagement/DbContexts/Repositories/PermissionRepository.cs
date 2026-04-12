using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;



internal sealed class PermissionRepository : IPermissionRepository
{
    private readonly UserManagementDbContext _context;

    public PermissionRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<Permission?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.Permissions.FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, ct);

    public async Task<IReadOnlyList<Permission>> GetPermissionsByRoleAsync(string roleId, CancellationToken ct = default)
    {
        var query = from rp in _context.RolePermissions
                    join p in _context.Permissions on rp.PermissionId equals p.Id
                    where rp.RoleId == roleId && p.DeletedAt == null
                    select p;
        return await query.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Permission>> GetPermissionsByUserAsync(string userId, string tenantId, CancellationToken ct = default)
    {
        // User -> TenantMembership -> Role -> RolePermissions -> Permission
        var query = from tm in _context.TenantMemberships
                    join rp in _context.RolePermissions on tm.RoleId equals rp.RoleId
                    join p in _context.Permissions on rp.PermissionId equals p.Id
                    where tm.UserId == userId && tm.TenantId == tenantId && tm.RevokedAt == null
                          && p.DeletedAt == null && (p.TenantId == tenantId || p.TenantId == "")
                    select p;
        return await query.Distinct().ToListAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetPermissionNamesByRoleAsync(string roleId, CancellationToken ct = default)
    {
        var query = from rp in _context.RolePermissions
                    join p in _context.Permissions on rp.PermissionId equals p.Id
                    where rp.RoleId == roleId && p.DeletedAt == null
                    select p.Name;
        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(Permission permission, CancellationToken ct = default)
    {
        await _context.Permissions.AddAsync(permission, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Permission permission, CancellationToken ct = default)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Permission permission, CancellationToken ct = default)
    {
        permission.DeletedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(permission, ct);
    }

    public async Task AddRolePermissionAsync(string roleId, string permissionId, CancellationToken ct = default)
    {
        var rp = new RolePermission { RoleId = roleId, PermissionId = permissionId };
        await _context.RolePermissions.AddAsync(rp, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveRolePermissionAsync(string roleId, string permissionId, CancellationToken ct = default)
    {
        var rp = await _context.RolePermissions
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.PermissionId == permissionId, ct);
        if (rp != null)
        {
            _context.RolePermissions.Remove(rp);
            await _context.SaveChangesAsync(ct);
        }
    }
}
