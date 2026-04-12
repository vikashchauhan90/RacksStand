using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RacksStands.Module.UserManagement.Operations.Auth.Signin;

internal sealed class SigninEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapPost("/auth/signin", Handler)
            .AllowAnonymous()
            .WithName("Signin")
            .WithSummary("Authenticate user with email/password and optional MFA");

    private static async Task<IResult> Handler(
        [FromBody] SigninRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var command = new SigninCommand(request.Email, request.Password, request.MfaCode);
        var result = await dispatcher.SendAsync<SigninCommand, Outcome<SigninResponse>>(command, context.RequestAborted);
        return result.ToIResult().ToHttpResult(context);
    }
}
