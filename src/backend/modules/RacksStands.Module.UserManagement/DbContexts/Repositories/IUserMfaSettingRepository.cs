namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IUserMfaSettingRepository
{
    Task<UserMfaSetting?> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task AddAsync(UserMfaSetting setting, CancellationToken ct = default);
    Task UpdateAsync(UserMfaSetting setting, CancellationToken ct = default);
    Task DeleteAsync(UserMfaSetting setting, CancellationToken ct = default);
    Task<bool> IsMfaEnabledAsync(string userId, CancellationToken ct = default);
}
