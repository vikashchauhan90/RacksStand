namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IProductRepository
{
    Task<Product?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetProductsByTenantAsync(string tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountProductsByTenantAsync(string tenantId, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task SoftDeleteAsync(Product product, CancellationToken ct = default);
}
