using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.Operations.Auth.Signin;
using System.Security.Cryptography;

internal sealed class SigninHandler(
    UserManagementDbContext dbContext,
    IJwtTokenService jwtTokenService,
    ILogger<SigninHandler> logger) : ICommandHandler<SigninCommand, Outcome<SigninResponse>>
{
    public async Task<Outcome<SigninResponse>> HandleAsync(SigninCommand command, CancellationToken ct)
    {
        // 1. Find user
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email && u.DeletedAt == null, ct);
        if (user == null || !PasswordHasher.VerifyHashedPassword(user.PasswordHash, command.Password))
            return Outcome<SigninResponse>.Unauthorized(new OutcomeError("Signin.InvalidCredentials", "Invalid email or password."));

        // 2. Check MFA
        var mfaSetting = await dbContext.UserMfaSettings
            .FirstOrDefaultAsync(m => m.UserId == user.Id && m.IsEnabled, ct);
        if (mfaSetting != null)
        {
            // MFA enabled – require code if not provided
            if (string.IsNullOrEmpty(command.MfaCode))
            {
                // Create a challenge and return MFA required response
                var challenge = new MfaChallenge
                {
                    Id = IdGenerator.NewGuidString(),
                    UserId = user.Id,
                    TokenHash = HashHelper.SHA256(RandomNumberGenerator.GetBytes(64).ToString()!),
                    ExpireAt = DateTimeOffset.UtcNow.AddMinutes(5),
                    CreatedAt = DateTimeOffset.UtcNow
                };
                await dbContext.MfaChallenges.AddAsync(challenge, ct);
                await dbContext.SaveChangesAsync(ct);

                return Outcome<SigninResponse>.Success(new SigninResponse(null!, 0, null!, true, challenge.Id));
            }

            // Verify MFA code
            if (!VerifyTotp(mfaSetting.TotpSecretEncrypted, command.MfaCode))
                return Outcome<SigninResponse>.Unauthorized(new OutcomeError("Signin.InvalidMfa", "Invalid MFA code."));

            // Mark any pending challenge as used (optional)
            var pendingChallenge = await dbContext.MfaChallenges
                .FirstOrDefaultAsync(c => c.UserId == user.Id && !c.IsUsed && c.ExpireAt > DateTimeOffset.UtcNow, ct);
            if (pendingChallenge != null)
            {
                pendingChallenge.IsUsed = true;
                pendingChallenge.UsedAt = DateTimeOffset.UtcNow;
                await dbContext.SaveChangesAsync(ct);
            }
        }

        // 3. Generate tokens
        var roles = await GetUserRoles(user.Id, ct);
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

        logger.LogInformation("User {UserId} signed in", user.Id);
        return Outcome<SigninResponse>.Success(new SigninResponse(
            accessToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            refreshToken,
            false));
    }

    private async Task<List<string>> GetUserRoles(string userId, CancellationToken ct)
    {
        // Join with TenantMembership + Role
        return await dbContext.TenantMemberships
            .Where(tm => tm.UserId == userId && tm.RevokedAt == null)
            .Join(dbContext.Roles, tm => tm.RoleId, r => r.Id, (_, r) => r.Name)
            .Distinct()
            .ToListAsync(ct);
    }

    private bool VerifyTotp(string encryptedSecret, string code)
    {
        // Decrypt secret (use a real encryption service – omitted for brevity)
        var secret = Decrypt(encryptedSecret);
        return TotpHasher.Verify(secret, code);
    }

    private string Decrypt(string encrypted) => encrypted; // Placeholder
}
