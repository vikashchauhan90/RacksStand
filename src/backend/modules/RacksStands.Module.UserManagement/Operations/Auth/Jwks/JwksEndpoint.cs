using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RacksStands.Framework.Auth.Authentication;

namespace RacksStands.Module.UserManagement.Operations.Auth.Jwks;

internal class JwksEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder group) =>
        group.MapGet("/.well-known/jwks.json", Handler)
            .WithName("Jwks")
            .WithSummary("JSON Web Key Set endpoint")
            .WithDescription("Provides public keys for verifying JWT signatures.")
            .WithTags("Auth")
            .Produces<JwksDocument>(StatusCodes.Status200OK)
            .AllowAnonymous();

    private static async Task<IResult> Handler(
        [FromServices]ISigningKeyFactory signingKeyFactory,
        HttpContext context)
    {
        var jwks = signingKeyFactory.GetJsonWebKeySet();
        context.Response.Headers.CacheControl = "public, max-age=300";
        return Results.Json(
            jwks,
            statusCode: StatusCodes.Status200OK);
    }
}
