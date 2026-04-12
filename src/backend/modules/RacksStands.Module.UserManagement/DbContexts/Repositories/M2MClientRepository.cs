using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;


internal sealed class M2MClientRepository : IM2MClientRepository
{
    private readonly UserManagementDbContext _context;

    public M2MClientRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<M2MClient?> GetByClientIdAsync(string clientId, CancellationToken ct = default)
        => await _context.M2MClients
            .FirstOrDefaultAsync(c => c.ClientId == clientId && c.DeletedAt == null, ct);

    public async Task<IReadOnlyList<M2MClient>> GetAllActiveAsync(CancellationToken ct = default)
        => await _context.M2MClients.Where(c => c.DeletedAt == null).ToListAsync(ct);

    public async Task AddAsync(M2MClient client, CancellationToken ct = default)
    {
        await _context.M2MClients.AddAsync(client, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(M2MClient client, CancellationToken ct = default)
    {
        _context.M2MClients.Update(client);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(M2MClient client, CancellationToken ct = default)
    {
        client.DeletedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(client, ct);
    }
}
