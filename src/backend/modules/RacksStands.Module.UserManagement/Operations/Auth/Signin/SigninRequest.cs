namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

public record SigninRequest(
    string Email,
    string Password
);
