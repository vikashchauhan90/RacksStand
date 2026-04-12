using RacksStands.Framework.Base.Hashers;
using RacksStands.Framework.Base.IdGenerators;
using RacksStands.Module.UserManagement.DbContexts.Repositories;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signup;

internal class SignupHandler(
    IUserRepository userRepository,
    ILogger<SignupHandler> logger
) : ICommandHandler<SignupCommand, Outcome<Unit>>
{
    public async Task<Outcome<Unit>> HandleAsync(SignupCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Checking if user already exists with email {Email} or username {UserName}", command.Email, command.UserName);

        // Check if user already exists
        if (await userRepository.ExistsByEmailAsync(command.Email, ct))
        {
            logger.LogWarning("User with email {Email} already exists", command.Email);
            return Outcome<Unit>.Conflict(new OutcomeError("Signup.AlreadyExists", "User with this email already exists."));
        }

        if (await userRepository.ExistsByUserNameAsync(command.UserName, ct))
        {
            logger.LogWarning("User with username {UserName} already exists", command.UserName);
            return Outcome<Unit>.Conflict(new OutcomeError("Signup.AlreadyExists", "User with this username already exists."));
        }

        var user = new User
        {
            Id = IdGenerator.NewGuidString(),
            Name = command.Name,
            UserName = command.UserName,
            Email = command.Email,
            PasswordHash = PasswordHasher.HashPassword(command.Password),
            CreatedAt = DateTimeOffset.UtcNow
        };

        await userRepository.AddAsync(user, ct);

        logger.LogInformation("User {UserName} created successfully with ID {UserId}", user.UserName, user.Id);
        logger.LogInformation("User {UserId} needs to create a tenant to start using the system", user.Id);

        return Outcome<Unit>.Success(Unit.Value);
    }
}
