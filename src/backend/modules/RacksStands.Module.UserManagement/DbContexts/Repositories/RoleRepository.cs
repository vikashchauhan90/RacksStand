using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;



internal sealed class RoleRepository : IRoleRepository
{
    private readonly UserManagementDbContext _context;

    public RoleRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<Role?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.Roles.FirstOrDefaultAsync(r => r.Id == id && r.DeletedAt == null, ct);

    public async Task<Role?> GetByNameAndTenantAsync(string name, string tenantId, CancellationToken ct = default)
        => await _context.Roles.FirstOrDefaultAsync(r => r.Name == name && r.TenantId == tenantId && r.DeletedAt == null, ct);

    public async Task<IReadOnlyList<Role>> GetRolesByTenantAsync(string tenantId, CancellationToken ct = default)
        => await _context.Roles.Where(r => r.TenantId == tenantId && r.DeletedAt == null).ToListAsync(ct);

    public async Task<IReadOnlyList<string>> GetRoleNamesByUserAsync(string userId, string tenantId, CancellationToken ct = default)
    {
        var query = from tm in _context.TenantMemberships
                    join r in _context.Roles on tm.RoleId equals r.Id
                    where tm.UserId == userId && tm.TenantId == tenantId && tm.RevokedAt == null && r.DeletedAt == null
                    select r.Name;
        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(Role role, CancellationToken ct = default)
    {
        await _context.Roles.AddAsync(role, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Role role, CancellationToken ct = default)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Role role, CancellationToken ct = default)
    {
        role.DeletedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(role, ct);
    }
}
