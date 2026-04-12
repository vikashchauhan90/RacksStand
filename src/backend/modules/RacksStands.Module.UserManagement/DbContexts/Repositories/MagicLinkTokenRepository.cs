using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;


internal sealed class MagicLinkTokenRepository : IMagicLinkTokenRepository
{
    private readonly UserManagementDbContext _context;

    public MagicLinkTokenRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<MagicLinkToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => await _context.MagicLinkTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash && t.DeletedAt == null, ct);

    public async Task<MagicLinkToken?> GetValidUnusedByUserIdAsync(string userId, CancellationToken ct = default)
        => await _context.MagicLinkTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsUsed && !t.IsRevoked && t.ExpireAt > DateTimeOffset.UtcNow && t.DeletedAt == null, ct);

    public async Task AddAsync(MagicLinkToken token, CancellationToken ct = default)
    {
        await _context.MagicLinkTokens.AddAsync(token, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task MarkUsedAsync(MagicLinkToken token, CancellationToken ct = default)
    {
        token.IsUsed = true;
        token.UsedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(token, ct);
    }

    public async Task RevokeAsync(MagicLinkToken token, CancellationToken ct = default)
    {
        token.IsRevoked = true;
        await UpdateAsync(token, ct);
    }

    public async Task RemoveExpiredAsync(CancellationToken ct = default)
    {
        var expired = await _context.MagicLinkTokens
            .Where(t => t.ExpireAt <= DateTimeOffset.UtcNow || t.DeletedAt != null)
            .ToListAsync(ct);
        _context.MagicLinkTokens.RemoveRange(expired);
        await _context.SaveChangesAsync(ct);
    }

    private async Task UpdateAsync(MagicLinkToken token, CancellationToken ct)
    {
        _context.MagicLinkTokens.Update(token);
        await _context.SaveChangesAsync(ct);
    }
}
