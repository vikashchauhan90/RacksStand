using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal sealed class TenantRepository : ITenantRepository
{
    private readonly UserManagementDbContext _context;

    public TenantRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<Tenant?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id && t.DeletedAt == null, ct);

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await _context.Tenants.FirstOrDefaultAsync(t => t.Slug == slug && t.DeletedAt == null, ct);

    public async Task<IReadOnlyList<Tenant>> GetTenantsByUserAsync(string userId, CancellationToken ct = default)
    {
        var query = from tm in _context.TenantMemberships
                    join t in _context.Tenants on tm.TenantId equals t.Id
                    where tm.UserId == userId && tm.RevokedAt == null && t.DeletedAt == null
                    select t;
        return await query.ToListAsync(ct);
    }

    public async Task AddAsync(Tenant tenant, CancellationToken ct = default)
    {
        await _context.Tenants.AddAsync(tenant, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken ct = default)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Tenant tenant, CancellationToken ct = default)
    {
        tenant.DeletedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(tenant, ct);
    }
}
