namespace RacksStands.Module.UserManagement.Operations.Auth.UserInfo;

public record UserInfoResponse(
    string Id,
    string Name,
    string UserName,
    string Email,
    List<string> Roles,
    List<string> Permissions
);
