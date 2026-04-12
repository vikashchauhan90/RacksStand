using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace RacksStands.Module.UserManagement.Operations.Auth.LogoutAll;

internal class LogoutAllEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapPost("/auth/logout-all", Handler)
            .WithName("LogoutAll")
            .WithSummary("Revoke all user sessions")
            .WithDescription("Revokes all refresh tokens for the current user, logging them out from all devices")
            .WithTags("Auth")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

    private static async Task<IResult> Handler(
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var command = new LogoutAllCommand();
        var result = await dispatcher.SendAsync<LogoutAllCommand, Outcome<Unit>>(command, context.RequestAborted);
        return result.ToIResult().ToHttpResult(context);
    }
}
