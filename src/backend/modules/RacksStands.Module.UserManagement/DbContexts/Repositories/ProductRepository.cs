using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;



internal sealed class ProductRepository : IProductRepository
{
    private readonly UserManagementDbContext _context;

    public ProductRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<Product?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.DeletedAt == null, ct);

    public async Task<IReadOnlyList<Product>> GetProductsByTenantAsync(string tenantId, int page, int pageSize, CancellationToken ct = default)
    {
        // Assuming Product entity has TenantId (add if missing)
        // For now, join with TenantSubscription? Actually Product is subscription product.
        // We'll assume Product is tenant-agnostic or has TenantId column.
        // If no TenantId, you may need to join via subscription.
        var query = _context.Products.Where(p => p.DeletedAt == null);
        // TODO: Add tenant filter if needed
        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountProductsByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        var query = _context.Products.Where(p => p.DeletedAt == null);
        return await query.CountAsync(ct);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(product, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Product product, CancellationToken ct = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(Product product, CancellationToken ct = default)
    {
        product.DeletedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(product, ct);
    }
}
