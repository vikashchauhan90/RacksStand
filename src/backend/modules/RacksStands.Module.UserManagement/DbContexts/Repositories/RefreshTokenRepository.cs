using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;



internal sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly UserManagementDbContext _context;

    public RefreshTokenRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && rt.DeletedAt == null, ct);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(string userId, CancellationToken ct = default)
        => await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpireAt > DateTimeOffset.UtcNow && rt.DeletedAt == null)
            .ToListAsync(ct);

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        await _context.RefreshTokens.AddAsync(refreshToken, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task RevokeAsync(RefreshToken refreshToken, CancellationToken ct = default)
    {
        refreshToken.IsRevoked = true;
        await UpdateAsync(refreshToken, ct);
    }

    public async Task RevokeAllForUserAsync(string userId, CancellationToken ct = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.DeletedAt == null)
            .ToListAsync(ct);
        foreach (var token in tokens)
            token.IsRevoked = true;
        await _context.SaveChangesAsync(ct);
    }

    public async Task RemoveExpiredAsync(CancellationToken ct = default)
    {
        var expired = await _context.RefreshTokens
            .Where(rt => rt.ExpireAt <= DateTimeOffset.UtcNow || rt.DeletedAt != null)
            .ToListAsync(ct);
        _context.RefreshTokens.RemoveRange(expired);
        await _context.SaveChangesAsync(ct);
    }

    private async Task UpdateAsync(RefreshToken refreshToken, CancellationToken ct)
    {
        _context.RefreshTokens.Update(refreshToken);
        await _context.SaveChangesAsync(ct);
    }
}
