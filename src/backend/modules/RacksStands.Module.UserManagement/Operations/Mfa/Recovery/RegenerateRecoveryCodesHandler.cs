using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Base.Hashers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Recovery;

// Operations/Mfa/Regenerate/RegenerateRecoveryCodesHandler.cs
internal sealed class RegenerateRecoveryCodesHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<RegenerateRecoveryCodesCommand, Outcome<string[]>>
{
    public async Task<Outcome<string[]>> HandleAsync(RegenerateRecoveryCodesCommand request, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var setting = await dbContext.UserMfaSettings.FirstOrDefaultAsync(s => s.UserId == userId, ct);
        if (setting == null)
            return Outcome<string[]>.NotFound(new OutcomeError("Mfa.NotConfigured", "MFA not configured."));

        var newCodes = TotpHasher.GenerateRecoveryCodes(10, 8);
        var hashedCodes = newCodes.Select(c => HashHelper.SHA256(c));
        setting.RecoveryCodeHash = string.Join(';', hashedCodes);
        await dbContext.SaveChangesAsync(ct);

        return Outcome<string[]>.Success(newCodes);
    }
}
