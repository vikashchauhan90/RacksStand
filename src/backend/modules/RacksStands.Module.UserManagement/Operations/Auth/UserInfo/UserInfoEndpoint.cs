using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace RacksStands.Module.UserManagement.Operations.Auth.UserInfo;

internal class UserInfoEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapGet("/auth/user-info", Handler)
            .WithName("UserInfo")
            .WithSummary("Get current user information")
            .WithDescription("Returns information about the currently authenticated user")
            .WithTags("Auth")
            .Produces<UserInfoResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();

    private static async Task<IResult> Handler(
        [FromServices] IQueryDispatcher dispatcher,
        HttpContext context)
    {
        var query = new UserInfoQuery();
        var result = await dispatcher.QueryAsync<UserInfoQuery, Outcome<UserInfoResponse>>(query, context.RequestAborted);
        return result.ToIResult().ToHttpResult(context);
    }
}
