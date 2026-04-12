namespace RacksStands.Module.UserManagement.Operations.Auth.MagicLink;

public record RequestMagicLinkCommand(string Email) : ICommand<Outcome<Unit>>;
