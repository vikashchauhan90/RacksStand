using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.DbContexts.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Invitations.Accept;

internal sealed class AcceptInvitationHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<AcceptInvitationCommand, Outcome<Unit>>
{
    public async Task<Outcome<Unit>> HandleAsync(AcceptInvitationCommand command, CancellationToken ct)
    {
        var tokenHash = HashHelper.SHA256(command.Token);
        var invitation = await dbContext.TenantInvitations
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash && i.Status == InvitationStatus.Pending && i.ExpireAt > DateTimeOffset.UtcNow, ct);
        if (invitation == null)
            return Outcome<Unit>.Problem(new OutcomeError("Invitation.InvalidOrExpired", "Invalid or expired invitation."));

        var currentUserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserId))
            return Outcome<Unit>.Unauthorized();

        // Create membership
        var membership = new TenantMembership
        {
            Id = IdGenerator.NewGuidString(),
            TenantId = invitation.TenantId,
            UserId = currentUserId,
            RoleId = invitation.RoleId,
            AssignedByUserId = invitation.InvitedByUserId,
            JoinedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.TenantMemberships.AddAsync(membership, ct);

        invitation.Status = InvitationStatus.Accepted;
        invitation.RespondedAt = DateTimeOffset.UtcNow;
        invitation.AcceptedByUserId = currentUserId;
        await dbContext.SaveChangesAsync(ct);

        return Outcome<Unit>.Success(Unit.Value);
    }
}
