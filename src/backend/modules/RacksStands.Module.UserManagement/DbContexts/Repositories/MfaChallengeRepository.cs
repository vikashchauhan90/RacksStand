using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;


internal sealed class MfaChallengeRepository : IMfaChallengeRepository
{
    private readonly UserManagementDbContext _context;

    public MfaChallengeRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<MfaChallenge?> GetByIdAsync(string id, CancellationToken ct = default)
        => await _context.MfaChallenges.FirstOrDefaultAsync(c => c.Id == id && c.DeletedAt == null, ct);

    public async Task<MfaChallenge?> GetValidUnusedByUserIdAsync(string userId, CancellationToken ct = default)
        => await _context.MfaChallenges
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsUsed && c.ExpireAt > DateTimeOffset.UtcNow && c.DeletedAt == null, ct);

    public async Task AddAsync(MfaChallenge challenge, CancellationToken ct = default)
    {
        await _context.MfaChallenges.AddAsync(challenge, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task MarkUsedAsync(MfaChallenge challenge, CancellationToken ct = default)
    {
        challenge.IsUsed = true;
        challenge.UsedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(challenge, ct);
    }

    public async Task RemoveExpiredAsync(CancellationToken ct = default)
    {
        var expired = await _context.MfaChallenges
            .Where(c => c.ExpireAt <= DateTimeOffset.UtcNow || c.DeletedAt != null)
            .ToListAsync(ct);
        _context.MfaChallenges.RemoveRange(expired);
        await _context.SaveChangesAsync(ct);
    }

    private async Task UpdateAsync(MfaChallenge challenge, CancellationToken ct)
    {
        _context.MfaChallenges.Update(challenge);
        await _context.SaveChangesAsync(ct);
    }
}
