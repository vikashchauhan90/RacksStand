using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Invitations.Generate;

public record GenerateInvitationCommand(string Email, string RoleId, int ExpiryDays = 7) : ICommand<Outcome<InvitationResponse>>;
public record InvitationResponse(string InvitationId, string InviteLink);
