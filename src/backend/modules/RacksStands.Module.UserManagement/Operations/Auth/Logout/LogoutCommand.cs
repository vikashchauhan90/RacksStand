namespace RacksStands.Module.UserManagement.Operations.Auth.Logout;

public record LogoutCommand(string RefreshToken) : ICommand<Outcome<Unit>>;
