namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IUserRepository
{
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetUsersByTenantAsync(string tenantId, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountUsersByTenantAsync(string tenantId, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task SoftDeleteAsync(User user, CancellationToken ct = default);
}
