
namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<IReadOnlyList<Tenant>> GetTenantsByUserAsync(string userId, CancellationToken ct = default);
    Task AddAsync(Tenant tenant, CancellationToken ct = default);
    Task UpdateAsync(Tenant tenant, CancellationToken ct = default);
    Task SoftDeleteAsync(Tenant tenant, CancellationToken ct = default);
}
