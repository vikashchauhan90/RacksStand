
namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IMfaChallengeRepository
{
    Task<MfaChallenge?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<MfaChallenge?> GetValidUnusedByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(MfaChallenge challenge, CancellationToken ct = default);
    Task MarkUsedAsync(MfaChallenge challenge, CancellationToken ct = default);
    Task RemoveExpiredAsync(CancellationToken ct = default);
}
