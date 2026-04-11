using Microsoft.EntityFrameworkCore;
using ResultifyCore;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signup;

internal class SignupHandler(
    UserManagementDbContext dbContext,
    ILogger<SignupHandler> logger
) : ICommandHandler<SignupCommand, Outcome<Unit>>
{
    public async Task<Outcome<Unit>> HandleAsync(SignupCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Fetching if user already exist.");
        // Check if user already exists
        var existingUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == command.Email || u.UserName == command.UserName, ct);

        if (existingUser != null)
        {
            logger.LogWarning("User with this email or username already exists.");
            return Outcome<Unit>.Conflict(new OutcomeError("Signup.AlreadyExists", "User with this email or username already exists."));
        }

        return Outcome<Unit>.Success(Unit.Value);
    }
}
