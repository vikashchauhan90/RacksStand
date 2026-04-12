namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IMagicLinkTokenRepository
{
    Task<MagicLinkToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task<MagicLinkToken?> GetValidUnusedByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(MagicLinkToken token, CancellationToken ct = default);
    Task MarkUsedAsync(MagicLinkToken token, CancellationToken ct = default);
    Task RevokeAsync(MagicLinkToken token, CancellationToken ct = default);
    Task RemoveExpiredAsync(CancellationToken ct = default);
}
