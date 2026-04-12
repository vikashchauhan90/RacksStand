
namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task RevokeAsync(RefreshToken refreshToken, CancellationToken ct = default);
    Task RevokeAllForUserAsync(string userId, CancellationToken ct = default);
    Task RemoveExpiredAsync(CancellationToken ct = default);
}
