using ResultifyCore;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

internal record SigninCommand(
    string Email,
    string Password,
    string? MfaCode = null
) : ICommand<Outcome<SigninResponse>>;
