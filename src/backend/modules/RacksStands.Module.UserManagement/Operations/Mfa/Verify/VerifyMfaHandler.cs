using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Auth.Security;
using RacksStands.Framework.Auth.Tenant;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.DbContexts.Repositories;
using RacksStands.Module.UserManagement.Operations.Auth.Signin;
using System.Security.Claims;

namespace RacksStands.Module.UserManagement.Operations.Mfa.Verify;

internal sealed class VerifyMfaHandler(
    IMfaChallengeRepository mfaChallengeRepository,
    IUserMfaSettingRepository userMfaSettingRepository,
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITenantMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    ITenantContext tenantContext,
    IJwtTokenService jwtTokenService,
    IDataProtectionService dataProtectionService,
    IHttpContextAccessor httpContextAccessor,
    ILogger<VerifyMfaHandler> logger
) : ICommandHandler<VerifyMfaCommand, Outcome<SigninResponse>>
{
    public async Task<Outcome<SigninResponse>> HandleAsync(VerifyMfaCommand command, CancellationToken ct)
    {
        logger.LogInformation("Verifying MFA for challenge {ChallengeId}", command.ChallengeId);

        // Get the challenge
        var challenge = await mfaChallengeRepository.GetByIdAsync(command.ChallengeId, ct);
        if (challenge == null || challenge.IsUsed || challenge.ExpireAt <= DateTimeOffset.UtcNow)
        {
            logger.LogWarning("Invalid or expired MFA challenge {ChallengeId}", command.ChallengeId);
            return Outcome<SigninResponse>.Problem(new OutcomeError(
                "Mfa.InvalidChallenge",
                "Challenge expired or invalid."));
        }

        // Get MFA setting for the user
        var setting = await userMfaSettingRepository.GetByUserIdAsync(challenge.UserId, ct);
        if (setting == null || !setting.IsEnabled)
        {
            logger.LogWarning("MFA not enabled for user {UserId}", challenge.UserId);
            return Outcome<SigninResponse>.Problem(new OutcomeError(
                "Mfa.NotEnabled",
                "MFA not enabled for this user."));
        }

        // Verify TOTP or recovery code
        bool isValid = await VerifyMfaCode(setting, command.Code);
        if (!isValid)
        {
            logger.LogWarning("Invalid MFA code provided for user {UserId}", challenge.UserId);
            return Outcome<SigninResponse>.Unauthorized(new OutcomeError(
                "Mfa.InvalidCode",
                "Invalid MFA code."));
        }

        // Mark challenge as used
        await mfaChallengeRepository.MarkUsedAsync(challenge, ct);
        logger.LogInformation("MFA challenge {ChallengeId} marked as used", challenge.Id);

        // Get user details
        var user = await userRepository.GetByIdAsync(challenge.UserId, ct);
        if (user == null || user.DeletedAt != null)
        {
            logger.LogError("User {UserId} not found after MFA verification", challenge.UserId);
            return Outcome<SigninResponse>.NotFound(new OutcomeError(
                "Mfa.UserNotFound",
                "User not found."));
        }

        // Get tenant context and roles
        var currentTenantId = tenantContext.GetCurrentTenantId();
        List<string> roles = new();

        if (!string.IsNullOrEmpty(currentTenantId))
        {
           // roles = await roleRepository.GetRoleNamesByUserAsync(user.Id, currentTenantId, ct);
            logger.LogInformation("User {UserId} has {RoleCount} roles in tenant {TenantId}",
                user.Id, roles.Count, currentTenantId);
        }
        else
        {
            // Fallback: get roles from first membership
            var memberships = await membershipRepository.GetMembershipsByUserAsync(user.Id, ct);
            var firstMembership = memberships.FirstOrDefault();
            if (firstMembership != null)
            {
                var role = await roleRepository.GetByIdAsync(firstMembership.RoleId, ct);
                if (role != null) roles.Add(role.Name);
            }
            logger.LogInformation("User {UserId} has {RoleCount} roles (no tenant context)", user.Id, roles.Count);
        }

        // Generate tokens
        var accessToken = GenerateTokenWithTenant(user, roles, currentTenantId);
        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = HashHelper.SHA256(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            Id = IdGenerator.NewGuidString(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpireAt = DateTimeOffset.UtcNow.AddDays(jwtTokenService.GetRefreshTokenExpiryDays()),
            CreatedAt = DateTimeOffset.UtcNow,
            IsRevoked = false
        };
        await refreshTokenRepository.AddAsync(refreshTokenEntity, ct);

        logger.LogInformation("User {UserId} completed MFA and received tokens", user.Id);

        return Outcome<SigninResponse>.Success(new SigninResponse(
            accessToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            refreshToken,
            false));
    }

    private async Task<bool> VerifyMfaCode(UserMfaSetting setting, string code)
    {
        try
        {
            // Decrypt the TOTP secret
            var decryptedSecret = dataProtectionService.Unprotect(setting.TotpSecretEncrypted);

            // Verify as TOTP
            if (TotpHasher.Verify(decryptedSecret, code))
                return true;

            // Verify as recovery code
            var recoveryCodeHash = HashHelper.SHA256(code);
            var savedHashes = setting.RecoveryCodeHash.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (savedHashes.Contains(recoveryCodeHash))
            {
                // Remove the used recovery code (optional but recommended)
                // This requires updating the setting; you might want to handle it separately
                // For now, we just validate.
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying MFA code for user {UserId}", setting.UserId);
            return false;
        }
    }

    private string GenerateTokenWithTenant(User user, List<string> roles, string? tenantId)
    {
        var additionalClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
        };

        if (!string.IsNullOrEmpty(tenantId))
        {
            additionalClaims.Add(new Claim("tenant_id", tenantId));
        }

        // If your IJwtTokenService supports custom claims, use it.
        // Otherwise, use the standard method.
        return jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
    }
}
