using RacksStands.Framework.Auth.Authentication;
using RacksStands.Framework.Auth.Security;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.DbContexts.Repositories;
using System.Security.Claims;
using System.Security.Cryptography;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

internal sealed class SigninHandler(
    IUserRepository userRepository,
    IUserMfaSettingRepository mfaSettingRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IMfaChallengeRepository mfaChallengeRepository,
    ITenantMembershipRepository membershipRepository,
    IRoleRepository roleRepository,
    IJwtTokenService jwtTokenService,
    IDataProtectionService dataProtectionService, // You'll need to implement this
    ILogger<SigninHandler> logger
) : ICommandHandler<SigninCommand, Outcome<SigninResponse>>
{
    public async Task<Outcome<SigninResponse>> HandleAsync(SigninCommand command, CancellationToken ct)
    {
        // 1. Find user
        var user = await userRepository.GetByEmailAsync(command.Email, ct);
        if (user == null || !PasswordHasher.VerifyHashedPassword(user.PasswordHash, command.Password))
        {
            logger.LogWarning("Failed login attempt for email {Email}", command.Email);
            return Outcome<SigninResponse>.Unauthorized(new OutcomeError("Signin.InvalidCredentials", "Invalid email or password."));
        }

        logger.LogInformation("User {UserId} authenticated successfully", user.Id);

        // 2. Check MFA
        var mfaSetting = await mfaSettingRepository.GetByUserIdAsync(user.Id, ct);
        if (mfaSetting != null && mfaSetting.IsEnabled)
        {
            // MFA enabled – require code if not provided
            if (string.IsNullOrEmpty(command.MfaCode))
            {
                // Create a challenge and return MFA required response
                var challenge = new MfaChallenge
                {
                    Id = IdGenerator.NewGuidString(),
                    UserId = user.Id,
                    TokenHash = GenerateRandomTokenHash(),
                    ExpireAt = DateTimeOffset.UtcNow.AddMinutes(5),
                    CreatedAt = DateTimeOffset.UtcNow
                };
                await mfaChallengeRepository.AddAsync(challenge, ct);

                logger.LogInformation("MFA challenge created for user {UserId}", user.Id);
                return Outcome<SigninResponse>.Success(new SigninResponse(null!, 0, null!, true, challenge.Id));
            }

            // Verify MFA code
            if (!await VerifyTotpAsync(mfaSetting.TotpSecretEncrypted, command.MfaCode))
            {
                logger.LogWarning("Invalid MFA code provided for user {UserId}", user.Id);
                return Outcome<SigninResponse>.Unauthorized(new OutcomeError("Signin.InvalidMfa", "Invalid MFA code."));
            }

            // Mark any pending challenge as used
            var pendingChallenge = await mfaChallengeRepository.GetValidUnusedByUserIdAsync(user.Id, ct);
            if (pendingChallenge != null)
            {
                await mfaChallengeRepository.MarkUsedAsync(pendingChallenge, ct);
                logger.LogInformation("MFA challenge {ChallengeId} marked as used", pendingChallenge.Id);
            }
        }

        // 3. Get user's default tenant and roles
        var memberships = await membershipRepository.GetMembershipsByUserAsync(user.Id, ct);
        string? defaultTenantId = null;
        List<string> roleNames = new();

        if (memberships.Any())
        {
            var activeMembership = memberships.First();
            defaultTenantId = activeMembership.TenantId;

            // Get role name for this membership
            var role = await roleRepository.GetByIdAsync(activeMembership.RoleId, ct);
            if (role != null)
                roleNames.Add(role.Name);

            logger.LogInformation("User {UserId} has default tenant {TenantId} with role {RoleName}",
                user.Id, defaultTenantId, role?.Name ?? "Unknown");
        }
        else
        {
            logger.LogInformation("User {UserId} has no tenants yet", user.Id);
        }

        // 4. Generate tokens
        var accessToken = GenerateTokenWithTenant(user, roleNames, defaultTenantId);
        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var refreshTokenHash = HashHelper.SHA256(refreshToken);

        var refreshTokenEntity = new DbContexts.Entities.RefreshToken
        {
            Id = IdGenerator.NewGuidString(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpireAt = DateTimeOffset.UtcNow.AddDays(jwtTokenService.GetRefreshTokenExpiryDays()),
            CreatedAt = DateTimeOffset.UtcNow
        };
        await refreshTokenRepository.AddAsync(refreshTokenEntity, ct);

        logger.LogInformation("User {UserId} signed in successfully. HasTenant: {HasTenant}",
            user.Id, defaultTenantId != null);

        return Outcome<SigninResponse>.Success(new SigninResponse(
            accessToken,
            jwtTokenService.GetAccessTokenExpirySeconds(),
            refreshToken,
            false,
            null));
    }

    private string GenerateTokenWithTenant(User user, List<string> roles, string? tenantId)
    {
        // Create additional claims
        var additionalClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name),
        };

        if (!string.IsNullOrEmpty(tenantId))
        {
            additionalClaims.Add(new Claim("tenant_id", tenantId));
        }

        // You'll need to extend IJwtTokenService to accept additional claims
        // For now, we'll use the existing method and add claims via a new method
        // Or modify the existing GenerateToken method to accept custom claims

        // Option 1: If you can modify IJwtTokenService
        // return jwtTokenService.GenerateTokenWithClaims(user.Id, user.UserName, roles, additionalClaims);

        // Option 2: Use existing method (tenant_id won't be included)
        return jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
    }

    private async Task<bool> VerifyTotpAsync(string encryptedSecret, string code)
    {
        try
        {
            // Unprotect the secret using your data protection service
            var secret = dataProtectionService.Unprotect(encryptedSecret);

            // Verify using TotpHasher
            return TotpHasher.Verify(secret, code);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error verifying TOTP code");
            return false;
        }
    }

    private string GenerateRandomTokenHash()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return HashHelper.SHA256(Convert.ToBase64String(randomBytes));
    }
}
