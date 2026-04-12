using Microsoft.EntityFrameworkCore;
using RacksStands.Module.UserManagement.DbContexts.Entities.Enums;

namespace RacksStands.Module.UserManagement.DbContexts.Repositories;


internal sealed class TenantInvitationRepository : ITenantInvitationRepository
{
    private readonly UserManagementDbContext _context;

    public TenantInvitationRepository(UserManagementDbContext context)
        => _context = context;

    public async Task<TenantInvitation?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
        => await _context.TenantInvitations
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash && i.DeletedAt == null, ct);

    public async Task<TenantInvitation?> GetPendingByIdAsync(string id, CancellationToken ct = default)
        => await _context.TenantInvitations
            .FirstOrDefaultAsync(i => i.Id == id && i.Status == InvitationStatus.Pending && i.ExpireAt > DateTimeOffset.UtcNow && i.DeletedAt == null, ct);

    public async Task<IReadOnlyList<TenantInvitation>> GetPendingByEmailAsync(string email, CancellationToken ct = default)
        => await _context.TenantInvitations
            .Where(i => i.Email == email && i.Status == InvitationStatus.Pending && i.ExpireAt > DateTimeOffset.UtcNow && i.DeletedAt == null)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<TenantInvitation>> GetInvitationsByTenantAsync(string tenantId, CancellationToken ct = default)
        => await _context.TenantInvitations
            .Where(i => i.TenantId == tenantId && i.DeletedAt == null)
            .ToListAsync(ct);

    public async Task AddAsync(TenantInvitation invitation, CancellationToken ct = default)
    {
        await _context.TenantInvitations.AddAsync(invitation, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TenantInvitation invitation, CancellationToken ct = default)
    {
        _context.TenantInvitations.Update(invitation);
        await _context.SaveChangesAsync(ct);
    }

    public async Task AcceptAsync(TenantInvitation invitation, string acceptedByUserId, CancellationToken ct = default)
    {
        invitation.Status = InvitationStatus.Accepted;
        invitation.AcceptedByUserId = acceptedByUserId;
        invitation.RespondedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(invitation, ct);
    }

    public async Task DeclineAsync(TenantInvitation invitation, CancellationToken ct = default)
    {
        invitation.Status = InvitationStatus.Declined;
        invitation.RespondedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(invitation, ct);
    }

    public async Task RevokeAsync(TenantInvitation invitation, CancellationToken ct = default)
    {
        invitation.Status = InvitationStatus.Revoked;
        invitation.RevokedAt = DateTimeOffset.UtcNow;
        await UpdateAsync(invitation, ct);
    }
}
