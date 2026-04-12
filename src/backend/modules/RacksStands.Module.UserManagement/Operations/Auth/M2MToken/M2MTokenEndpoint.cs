// Operations/Auth/M2MToken/M2MTokenEndpoint.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RacksStands.Module.UserManagement.Operations.Auth.M2MToken;

internal sealed class M2MTokenEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapPost("/auth/m2m-token", Handler)
            .AllowAnonymous()
            .WithName("M2MToken");

    private static async Task<IResult> Handler(
        [FromBody] M2MTokenRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var command = new M2MTokenCommand(request.ClientId, request.ClientSecret, request.Scope);
        var result = await dispatcher.SendAsync<M2MTokenCommand, Outcome<M2MTokenResponse>>(command, context.RequestAborted);
        return result.ToIResult().ToHttpResult(context);
    }
}


