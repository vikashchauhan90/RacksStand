using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Invitations.Accept;

public record AcceptInvitationCommand(string Token) : ICommand<Outcome<Unit>>;
