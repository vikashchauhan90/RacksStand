using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ResultifyCore;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace RacksStands.Module.UserManagement.Operations.Auth.Logout;
internal class LogoutEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapPost("/auth/logout", Handler)
            .WithName("Logout")
            .WithSummary("User logout")
            .WithDescription("Revokes the current refresh token")
            .WithTags("Auth")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

    private static async Task<IResult> Handler(
        [FromBody] LogoutRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var command = new LogoutCommand(request.RefreshToken);
        var result = await dispatcher.SendAsync<LogoutCommand, Outcome<Unit>>(command, context.RequestAborted);
        return result.ToIResult().ToHttpResult(context);
    }
}
