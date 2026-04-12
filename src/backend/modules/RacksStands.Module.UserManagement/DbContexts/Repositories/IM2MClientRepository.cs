namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface IM2MClientRepository
{
    Task<M2MClient?> GetByClientIdAsync(string clientId, CancellationToken ct = default);
    Task<IReadOnlyList<M2MClient>> GetAllActiveAsync(CancellationToken ct = default);
    Task AddAsync(M2MClient client, CancellationToken ct = default);
    Task UpdateAsync(M2MClient client, CancellationToken ct = default);
    Task SoftDeleteAsync(M2MClient client, CancellationToken ct = default);
}
