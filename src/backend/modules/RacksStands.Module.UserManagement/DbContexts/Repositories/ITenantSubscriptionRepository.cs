namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface ITenantSubscriptionRepository
{
    Task<TenantSubscription?> GetActiveByTenantAsync(string tenantId, CancellationToken ct = default);
    Task<IReadOnlyList<TenantSubscription>> GetSubscriptionsByTenantAsync(string tenantId, CancellationToken ct = default);
    Task AddAsync(TenantSubscription subscription, CancellationToken ct = default);
    Task UpdateAsync(TenantSubscription subscription, CancellationToken ct = default);
    Task CancelAsync(TenantSubscription subscription, CancellationToken ct = default);
}
