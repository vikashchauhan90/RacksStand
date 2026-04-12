using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace RacksStands.Module.UserManagement.Operations.Auth.RefreshToken;


internal class RefreshTokenEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapPost("/auth/refresh", Handler)
            .WithName("RefreshToken")
            .WithSummary("Refresh access token")
            .WithDescription("Uses a valid refresh token to obtain new access and refresh tokens")
            .WithTags("Auth")
            .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AllowAnonymous();

    private static async Task<IResult> Handler(
        [FromBody] RefreshTokenRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await dispatcher.SendAsync<RefreshTokenCommand, Outcome<RefreshTokenResponse>>(command, context.RequestAborted);
        return result.ToIResult().ToHttpResult(context);
    }
}
