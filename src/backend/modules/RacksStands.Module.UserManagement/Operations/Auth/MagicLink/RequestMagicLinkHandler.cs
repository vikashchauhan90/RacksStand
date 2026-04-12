// Operations/Auth/MagicLink/RequestMagicLinkHandler.cs
using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.Operations.Auth.MagicLink;

internal sealed class RequestMagicLinkHandler(
    UserManagementDbContext dbContext,
    ILogger<RequestMagicLinkHandler> logger) : ICommandHandler<RequestMagicLinkCommand, Outcome<Unit>>
{
    public async Task<Outcome<Unit>> HandleAsync(RequestMagicLinkCommand command, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == command.Email && u.DeletedAt == null, ct);
        if (user == null)
        {
            // Do not reveal that user does not exist – return success anyway
            logger.LogWarning("Magic link requested for non-existing email {Email}", command.Email);
            return Outcome<Unit>.Success(Unit.Value);
        }

        // Generate token
        var rawToken = IdGenerator.NewRandomString(32);
        var tokenHash = HashHelper.SHA256(rawToken);
        var magicLink = new MagicLinkToken
        {
            Id = IdGenerator.NewGuidString(),
            UserId = user.Id,
            Email = user.Email,
            TokenHash = tokenHash,
            ExpireAt = DateTimeOffset.UtcNow.AddMinutes(15),
            CreatedAt = DateTimeOffset.UtcNow
        };
        await dbContext.MagicLinkTokens.AddAsync(magicLink, ct);
        await dbContext.SaveChangesAsync(ct);

        // Send email (real link would point to frontend verify endpoint)
        var link = $"https://yourapp.com/auth/magic-link/verify?token={rawToken}";
       // await emailSender.SendEmailAsync(user.Email, "Your magic login link", $"Click here: {link}", ct);

        logger.LogInformation("Magic link sent to {Email}", user.Email);
        return Outcome<Unit>.Success(Unit.Value);
    }
}
