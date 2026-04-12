using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.Operations.Auth.Signin;
using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Verify;

// Operations/Mfa/Verify/VerifyMfaHandler.cs
internal sealed class VerifyMfaHandler(
    UserManagementDbContext dbContext,
    IJwtTokenService jwtTokenService,
    IHttpContextAccessor httpContextAccessor) : ICommandHandler<VerifyMfaCommand, Outcome<SigninResponse>>
{
    public async Task<Outcome<SigninResponse>> HandleAsync(VerifyMfaCommand command, CancellationToken ct)
    {
        var challenge = await dbContext.MfaChallenges
            .FirstOrDefaultAsync(c => c.Id == command.ChallengeId && !c.IsUsed && c.ExpireAt > DateTimeOffset.UtcNow, ct);
        if (challenge == null)
            return Outcome<SigninResponse>.Problem(new OutcomeError("Mfa.InvalidChallenge", "Challenge expired or invalid."));

        var setting = await dbContext.UserMfaSettings
            .FirstOrDefaultAsync(s => s.UserId == challenge.UserId && s.IsEnabled, ct);
        if (setting == null)
            return Outcome<SigninResponse>.Problem(new OutcomeError("Mfa.NotEnabled", "MFA not enabled."));

        // Verify TOTP or recovery code
        bool isValid = TotpHasher.Verify(Decrypt(setting.TotpSecretEncrypted), command.Code) ||
                       setting.RecoveryCodeHash.Split(';').Any(h => h == HashHelper.SHA256(command.Code));
        if (!isValid)
            return Outcome<SigninResponse>.Unauthorized(new OutcomeError("Mfa.InvalidCode", "Invalid MFA code."));

        // Mark challenge used
        challenge.IsUsed = true;
        challenge.UsedAt = DateTimeOffset.UtcNow;

        // Generate tokens
        var user = await dbContext.Users.FindAsync(new object[] { challenge.UserId }, ct);
        var roles = await GetUserRoles(user!.Id, ct);
        var accessToken = jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = HashHelper.SHA256(refreshToken);
        var refreshTokenEntity = new RefreshToken
        {
            Id = IdGenerator.NewGuidString(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpireAt = DateTimeOffset.UtcNow.AddDays(jwtTokenService.GetRefreshTokenExpiryDays()),
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.RefreshTokens.AddAsync(refreshTokenEntity, ct);
        await dbContext.SaveChangesAsync(ct);

        return Outcome<SigninResponse>.Success(new SigninResponse(
            accessToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            refreshToken,
            false));
    }
}
