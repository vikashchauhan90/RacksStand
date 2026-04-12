
namespace RacksStands.Module.UserManagement.DbContexts.Repositories;

internal interface ITenantInvitationRepository
{
    Task<TenantInvitation?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
    Task<TenantInvitation?> GetPendingByIdAsync(string id, CancellationToken ct = default);
    Task<IReadOnlyList<TenantInvitation>> GetPendingByEmailAsync(string email, CancellationToken ct = default);
    Task<IReadOnlyList<TenantInvitation>> GetInvitationsByTenantAsync(string tenantId, CancellationToken ct = default);
    Task AddAsync(TenantInvitation invitation, CancellationToken ct = default);
    Task UpdateAsync(TenantInvitation invitation, CancellationToken ct = default);
    Task AcceptAsync(TenantInvitation invitation, string acceptedByUserId, CancellationToken ct = default);
    Task DeclineAsync(TenantInvitation invitation, CancellationToken ct = default);
    Task RevokeAsync(TenantInvitation invitation, CancellationToken ct = default);
}
