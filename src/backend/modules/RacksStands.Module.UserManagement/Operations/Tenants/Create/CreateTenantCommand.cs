using System;
using System.Collections.Generic;
using System.Text;

namespace RacksStands.Module.UserManagement.Operations.Tenants.Create;

public record CreateTenantCommand(string Name, string Slug) : ICommand<Outcome<TenantResponse>>;
