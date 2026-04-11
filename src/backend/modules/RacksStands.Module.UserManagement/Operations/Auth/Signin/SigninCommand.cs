using ResultifyCore;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

internal record SigninCommand(
    string Email,
    string Password
) : ICommand<Outcome<SigninResponse>>;
