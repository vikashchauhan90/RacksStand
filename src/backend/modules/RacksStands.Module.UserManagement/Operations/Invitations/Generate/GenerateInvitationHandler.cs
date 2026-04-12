using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Auth.Tenant;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.DbContexts.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Invitations.Generate;

// Operations/Invitations/Generate/GenerateInvitationHandler.cs
internal sealed class GenerateInvitationHandler(
    UserManagementDbContext dbContext,
    ITenantContext tenantContext,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<GenerateInvitationCommand, Outcome<InvitationResponse>>
{
    public async Task<Outcome<InvitationResponse>> HandleAsync(GenerateInvitationCommand command, CancellationToken ct)
    {
        var currentUserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var tenantId = tenantContext.GetCurrentTenantId();
        if (string.IsNullOrEmpty(tenantId))
            return Outcome<InvitationResponse>.Problem(new OutcomeError("Invitation.TenantRequired", "Tenant not resolved."));

        // Check that the current user has permission to invite (role check omitted)

        var rawToken = IdGenerator.NewRandomString(32);
        var tokenHash = HashHelper.SHA256(rawToken);
        var invitation = new TenantInvitation
        {
            Id = IdGenerator.NewGuidString(),
            TenantId = tenantId,
            RoleId = command.RoleId,
            Email = command.Email,
            TokenHash = tokenHash,
            InvitedByUserId = currentUserId,
            ExpireAt = DateTimeOffset.UtcNow.AddDays(command.ExpiryDays),
            Status = InvitationStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.TenantInvitations.AddAsync(invitation, ct);
        await dbContext.SaveChangesAsync(ct);

        var inviteLink = $"https://yourapp.com/invite/accept?token={rawToken}";
       // await emailSender.SendEmailAsync(command.Email, "You are invited to join a tenant", $"Click here: {inviteLink}", ct);

        return Outcome<InvitationResponse>.Success(new InvitationResponse(invitation.Id, inviteLink));
    }
}
