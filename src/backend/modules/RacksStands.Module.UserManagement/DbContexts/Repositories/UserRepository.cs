using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;


internal sealed class UserRepository : IUserRepository
{
    private readonly UserManagementDbContext _context;

    public UserRepository(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.DeletedAt == null, ct);

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default)
        => await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.DeletedAt == null, ct);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _context.Users.AnyAsync(u => u.Email == email && u.DeletedAt == null, ct);

    public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
        => await _context.Users.AnyAsync(u => u.UserName == userName && u.DeletedAt == null, ct);

    public async Task<IReadOnlyList<User>> GetUsersByTenantAsync(string tenantId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = from tm in _context.TenantMemberships
                    join u in _context.Users on tm.UserId equals u.Id
                    where tm.TenantId == tenantId && tm.RevokedAt == null && u.DeletedAt == null
                    select u;

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountUsersByTenantAsync(string tenantId, CancellationToken ct = default)
    {
        return await _context.TenantMemberships
            .Where(tm => tm.TenantId == tenantId && tm.RevokedAt == null)
            .Join(_context.Users, tm => tm.UserId, u => u.Id, (_, u) => u)
            .CountAsync(u => u.DeletedAt == null, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(ct);
    }

    public async Task SoftDeleteAsync(User user, CancellationToken ct = default)
    {
        user.DeletedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(user, ct);
    }
}
