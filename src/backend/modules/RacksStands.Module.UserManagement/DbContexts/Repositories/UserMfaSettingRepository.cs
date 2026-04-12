using Microsoft.EntityFrameworkCore;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;



internal sealed class UserMfaSettingRepository : IUserMfaSettingRepository
{
    private readonly UserManagementDbContext _context;

    public UserMfaSettingRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<UserMfaSetting?> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => await _context.UserMfaSettings
            .FirstOrDefaultAsync(s => s.UserId == userId && s.DeletedAt == null, ct);

    public async Task AddAsync(UserMfaSetting setting, CancellationToken ct = default)
    {
        await _context.UserMfaSettings.AddAsync(setting, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(UserMfaSetting setting, CancellationToken ct = default)
    {
        _context.UserMfaSettings.Update(setting);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(UserMfaSetting setting, CancellationToken ct = default)
    {
        setting.DeletedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(setting, ct);
    }

    public async Task<bool> IsMfaEnabledAsync(string userId, CancellationToken ct = default)
    {
        var setting = await GetByUserIdAsync(userId, ct);
        return setting != null && setting.IsEnabled;
    }
}
