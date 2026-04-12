using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Base.Hashers;
using ResultifyCore;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

internal class SigninHandler(
    UserManagementDbContext dbContext,
    ILogger<SigninHandler> logger
) : ICommandHandler<SigninCommand, Outcome<SigninResponse>>
{
    public async Task<Outcome<SigninResponse>> HandleAsync(SigninCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Attempting signin for user with email {Email}", command.Email);

        // Find user by email
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email && u.DeletedAt == null, ct);

        if (user == null)
        {
            logger.LogWarning("User with email {Email} not found", command.Email);
            return Outcome<SigninResponse>.Unauthorized(new OutcomeError(
                "Signin.InvalidCredentials",
                "Invalid email or password."));
        }

        // Verify password
        if (!PasswordHasher.VerifyHashedPassword(user.PasswordHash, command.Password))
        {
            logger.LogWarning("Invalid password attempt for user {Email}", command.Email);
            return Outcome<SigninResponse>.Unauthorized(new OutcomeError(
                "Signin.InvalidCredentials",
                "Invalid email or password."));
        }

        // Get roles and permissions
        var roles = await GetUserRoles(user.Id, ct);
        var permissions = await GetUserPermissions(user.Id, ct);

        // Generate tokens
        //var accessToken = jwtTokenService.GenerateToken(user.Id, user.UserName, roles);
        //var refreshToken = jwtTokenService.GenerateRefreshToken();

        //// Revoke existing refresh tokens (optional - security)
        //var existingTokens = await dbContext.RefreshTokens
        //    .Where(rt => rt.UserId == user.Id && !rt.IsRevoked && rt.ExpireAt > DateTime.UtcNow)
        //    .ToListAsync(ct);

        //foreach (var token in existingTokens)
        //{
        //    token.IsRevoked = true;
        //}

        // Store new refresh token
        //var refreshTokenEntity = new RefreshToken
        //{
        //    Id = IdGenerator.NewGuidString(),
        //    UserId = user.Id,
        //    TokenHash = refreshToken,
        //    ExpireAt = DateTime.UtcNow.AddDays(7),
        //    CreatedAt = DateTime.UtcNow
        //};
        //await dbContext.RefreshTokens.AddAsync(refreshTokenEntity, ct);
        //await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserName} signed in successfully", user.UserName);

        //var response = new SigninResponse(
        //    accessToken,
        //    refreshToken,
        //    jwtTokenService.GetAccessTokenExpirySeconds(),
        //    "Bearer"
        //);

        return Outcome<SigninResponse>.Success(null);
    }

    private async Task<List<string>> GetUserRoles(string userId, CancellationToken ct)
    {
        return await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync(ct);
    }

    private async Task<List<string>> GetUserPermissions(string userId, CancellationToken ct)
    {
        return await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Join(dbContext.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp)
            .Join(dbContext.Permissions, rp => rp.PermissionId, p => p.Id, (rp, p) => p.Name)
            .Distinct()
            .ToListAsync(ct);
    }
}
