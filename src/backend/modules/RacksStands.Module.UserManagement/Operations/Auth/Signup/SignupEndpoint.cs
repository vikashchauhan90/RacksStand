using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RacksStands.Module.UserManagement.Operations.Auth.Signup;

internal class SignupEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapPost("/auth/signup", Handler)
            .WithName("Signup")
            .WithSummary("User registration")
            .WithDescription("Creates a new user account")
            .WithTags("Auth")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

    private static async Task<IResult> Handler(
        [FromBody] SignupRequest request,
        [FromServices] ICommandDispatcher dispatcher,
        HttpContext context)
    {
        var command = new SignupCommand(
            request.Name,
            request.UserName,
            request.Email,
            request.Password
        );

        var result = await dispatcher.SendAsync<SignupCommand, Outcome<Unit>>(command, context.RequestAborted);
        return result.ToIResult().ToHttpResult(context);
    }
}
