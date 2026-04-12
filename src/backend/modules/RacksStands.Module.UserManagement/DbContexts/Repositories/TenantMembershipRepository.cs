using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal sealed class TenantMembershipRepository : ITenantMembershipRepository
{
    private readonly UserManagementDbContext _context;

    public TenantMembershipRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<TenantMembership?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.TenantMemberships.FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<TenantMembership?> GetByUserAndTenantAsync(string userId, string tenantId, CancellationToken ct = default)
        => await _context.TenantMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.TenantId == tenantId && m.RevokedAt == null, ct);

    public async Task<IReadOnlyList<TenantMembership>> GetMembershipsByUserAsync(string userId, CancellationToken ct = default)
        => await _context.TenantMemberships
            .Where(m => m.UserId == userId && m.RevokedAt == null)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<TenantMembership>> GetMembershipsByTenantAsync(string tenantId, CancellationToken ct = default)
        => await _context.TenantMemberships
            .Where(m => m.TenantId == tenantId && m.RevokedAt == null)
            .ToListAsync(ct);

    public async Task AddAsync(TenantMembership membership, CancellationToken ct = default)
    {
        await _context.TenantMemberships.AddAsync(membership, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TenantMembership membership, CancellationToken ct = default)
    {
        _context.TenantMemberships.Update(membership);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(TenantMembership membership, CancellationToken ct = default)
    {
        membership.RevokedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(membership, ct);
    }
}
