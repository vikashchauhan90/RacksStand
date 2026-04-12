using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal sealed class TenantSubscriptionRepository : ITenantSubscriptionRepository
{
    private readonly UserManagementDbContext _context;

    public TenantSubscriptionRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<TenantSubscription?> GetActiveByTenantAsync(string tenantId, CancellationToken ct = default)
        => await _context.TenantSubscriptions
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && (s.ExpireAt == null || s.ExpireAt > DateTimeOffset.UtcNow) && s.DeletedAt == null, ct);

    public async Task<IReadOnlyList<TenantSubscription>> GetSubscriptionsByTenantAsync(string tenantId, CancellationToken ct = default)
        => await _context.TenantSubscriptions
            .Where(s => s.TenantId == tenantId && s.DeletedAt == null)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync(ct);

    public async Task AddAsync(TenantSubscription subscription, CancellationToken ct = default)
    {
        await _context.TenantSubscriptions.AddAsync(subscription, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TenantSubscription subscription, CancellationToken ct = default)
    {
        _context.TenantSubscriptions.Update(subscription);
        await _context.SaveChangesAsync(ct);
    }

    public async Task CancelAsync(TenantSubscription subscription, CancellationToken ct = default)
    {
        // Soft cancel by setting ExpireAt to now or marking deleted
        subscription.ExpireAt = DateTimeOffset.UtcNow;
        await UpdateAsync(subscription, ct);
    }
}
