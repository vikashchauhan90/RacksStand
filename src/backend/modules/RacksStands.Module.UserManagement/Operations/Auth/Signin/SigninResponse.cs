namespace RacksStands.Module.UserManagement.Operations.Auth.Signin;

public record SigninResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType
);
