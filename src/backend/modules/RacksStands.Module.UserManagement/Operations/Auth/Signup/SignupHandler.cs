using Microsoft.EntityFrameworkCore;
using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.Operations.Auth.Signin;
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

        // Get default role
        var defaultRole = await dbContext.Roles
            .FirstOrDefaultAsync(r => r.Name == "User", ct);

        // Create new user
        var user = new User
        {
            Id = IdGenerator.NewGuidString(),
            Name = command.Name,
            UserName = command.UserName,
            Email = command.Email,
            PasswordHash = PasswordHasher.HashPassword(command.Password),
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Users.AddAsync(user, ct);
        // Assign default role if exists
        //if (defaultRole != null)
        //{
        //    var userRole = new Role { UserId = user.Id, RoleId = defaultRole.Id };
        //    await dbContext.UserRoles.AddAsync(userRole, ct);
        //}

        await dbContext.SaveChangesAsync(ct);

        logger.LogInformation("User {UserName} created successfully with ID {UserId}", user.UserName, user.Id);

        return Outcome<Unit>.Success(Unit.Value);
    }
}
