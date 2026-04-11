namespace RacksStands.Module.UserManagement.Operations.Auth.Signup;

public record SignupRequest(
    string Name,
    string UserName,
    string Email,
    string Password
);
