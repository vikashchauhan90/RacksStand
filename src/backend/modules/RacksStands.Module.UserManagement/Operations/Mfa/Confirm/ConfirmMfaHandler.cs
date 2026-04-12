using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Confirm;

// Operations/Mfa/Confirm/ConfirmMfaHandler.cs
internal sealed class ConfirmMfaHandler(
    UserManagementDbContext dbContext,
    IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider dataProtection) : ICommandHandler<ConfirmMfaCommand, Outcome<ConfirmMfaResponse>>
{
    public async Task<Outcome<ConfirmMfaResponse>> HandleAsync(ConfirmMfaCommand command, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Outcome<ConfirmMfaResponse>.Unauthorized();

        // Validate TOTP code against the provided secret (temporary)
        if (!TotpHasher.Verify(command.TotpSecret!, command.TotpCode))
            return Outcome<ConfirmMfaResponse>.Problem(new OutcomeError("Mfa.InvalidCode", "Invalid TOTP code."));

        // Encrypt and store secret
        var encryptedSecret = dataProtection.Protect(command.TotpSecret!);
        var recoveryCodes = TotpHasher.GenerateRecoveryCodes(10, 8);
        var recoveryCodeHashes = recoveryCodes.Select(c => HashHelper.SHA256(c)).ToList();

        var setting = new UserMfaSetting
        {
            Id = IdGenerator.NewGuidString(),
            UserId = userId,
            TotpSecretEncrypted = encryptedSecret,
            RecoveryCodeHash = string.Join(';', recoveryCodeHashes),
            IsEnabled = true,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.UserMfaSettings.AddAsync(setting, ct);
        await dbContext.SaveChangesAsync(ct);

        return Outcome<ConfirmMfaResponse>.Success(new ConfirmMfaResponse(recoveryCodes));
    }
}
