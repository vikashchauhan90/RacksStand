using ResultifyCore;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signup;

internal record SignupCommand(
    string Name,
    string UserName,
    string Email,
    string Password
) : ICommand<Outcome<Unit>>;
