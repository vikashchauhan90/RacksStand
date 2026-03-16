using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Base.Serializers;
using RacksStandsResult = RacksStands.Framework.Results;
using HttpResults = Microsoft.AspNetCore.Http.Results;
using RacksStands.Framework.Hal;

namespace RacksStands.Framework.Modules.Bootstrap.Extensions;

public static class HttpResultExtensions
{
    public static IResult ToHttpResult(this RacksStandsResult.IResult domainResult, HttpContext httpContext)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        if (domainResult.Status == RacksStandsResult.ResultState.NoContent)
        {
            return TypedResults.NoContent();
        }


        if (domainResult.Status == RacksStandsResult.ResultState.Success)
        {
            return TypedResults.Ok();
        }

        if (domainResult.Status == RacksStandsResult.ResultState.Created)
        {
            return TypedResults.Created();
        }

        var errorSerialized = ResponseSerializer.Serialize(domainResult.Errors, contentType);

        if (contentType == ContentType.Json)
        {
            return HttpResults.Json(
                errorSerialized,
                statusCode: MapStatusToHttpCode(domainResult.Status)
                );
        }

        return HttpResults.Text(
            errorSerialized,
            ContentTypes.ToMime(contentType),
            MapStatusToHttpCode(domainResult.Status)
        );
    }
    public static IResult ToHttpResult<T>(this RacksStandsResult.IResult<T> domainResult, HttpContext httpContext)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        // Success branch
        if (domainResult.Status == RacksStandsResult.ResultState.Success ||
            domainResult.Status == RacksStandsResult.ResultState.Created ||
            domainResult.Status == RacksStandsResult.ResultState.NoContent)
        {
            if (domainResult.Status == RacksStandsResult.ResultState.NoContent)
            {
                return TypedResults.NoContent();
            }

            var serialized = domainResult.Data is not null ?
                ResponseSerializer.Serialize(domainResult.Data, contentType)
                : [];

            if (contentType == ContentType.Json)
            {
                return HttpResults.Json(
                    serialized,
                    statusCode: MapStatusToHttpCode(domainResult.Status)
                    );
            }

            return HttpResults.Text(
                serialized,
                ContentTypes.ToMime(contentType),
                MapStatusToHttpCode(domainResult.Status)
            );
        }

        var errorSerialized = ResponseSerializer.Serialize(domainResult.Errors, contentType);

        if (contentType == ContentType.Json)
        {
            return HttpResults.Json(
                errorSerialized,
                statusCode: MapStatusToHttpCode(domainResult.Status)
                );
        }

        return HttpResults.Text(
            errorSerialized,
            ContentTypes.ToMime(contentType),
            MapStatusToHttpCode(domainResult.Status)
        );
    }
    public static IResult ToApiResult(this RacksStandsResult.IResult domainResult, HttpContext httpContext, string? errorMessage = null)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        if (domainResult.Status == RacksStandsResult.ResultState.NoContent)
        {
            return TypedResults.NoContent();
        }


        if (domainResult.Status == RacksStandsResult.ResultState.Success)
        {
            return TypedResults.Ok(new ApiResponse { Success = true });
        }

        if (domainResult.Status == RacksStandsResult.ResultState.Created)
        {
            return TypedResults.Created();
        }

        var errorSerialized = ResponseSerializer.Serialize(new ApiResponse
        {
            Success = false,
            Error = errorMessage,
            ErrorMeta = domainResult.Errors

        },
        contentType);

        if (contentType == ContentType.Json)
        {
            return HttpResults.Json(
                errorSerialized,
                statusCode: MapStatusToHttpCode(domainResult.Status)
                );
        }

        return HttpResults.Text(
            errorSerialized,
            ContentTypes.ToMime(contentType),
            MapStatusToHttpCode(domainResult.Status)
        );
    }
    public static IResult ToApiResult<T>(this RacksStandsResult.IResult<T> domainResult, HttpContext httpContext, string? errorMessage = null)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        // Success branch
        if (domainResult.Status == RacksStandsResult.ResultState.Success ||
            domainResult.Status == RacksStandsResult.ResultState.Created ||
            domainResult.Status == RacksStandsResult.ResultState.NoContent)
        {
            if (domainResult.Status == RacksStandsResult.ResultState.NoContent)
            {
                return TypedResults.NoContent();
            }

            var serialized = ResponseSerializer.Serialize(new ApiResponse<T>
            {
                Success = true,
                Data = domainResult.Data
            }, contentType);

            if (contentType == ContentType.Json)
            {
                return HttpResults.Json(
                    serialized,
                    statusCode: MapStatusToHttpCode(domainResult.Status)
                    );
            }

            return HttpResults.Text(
                serialized,
                ContentTypes.ToMime(contentType),
                MapStatusToHttpCode(domainResult.Status)
            );
        }

        var errorSerialized = ResponseSerializer.Serialize(new ApiResponse<T>
        {
            Success = true,
            Data = domainResult.Data,
            Error = errorMessage,
            ErrorMeta = domainResult.Errors
        }, contentType);

        if (contentType == ContentType.Json)
        {
            return HttpResults.Json(
                errorSerialized,
                statusCode: MapStatusToHttpCode(domainResult.Status)
                );
        }

        return HttpResults.Text(
            errorSerialized,
            ContentTypes.ToMime(contentType),
            MapStatusToHttpCode(domainResult.Status)
        );
    }
    private static int MapStatusToHttpCode(RacksStandsResult.ResultState state) => state switch
    {
        RacksStandsResult.ResultState.Success => StatusCodes.Status200OK,
        RacksStandsResult.ResultState.Created => StatusCodes.Status201Created,
        RacksStandsResult.ResultState.NoContent => StatusCodes.Status204NoContent,
        RacksStandsResult.ResultState.NotFound => StatusCodes.Status404NotFound,
        RacksStandsResult.ResultState.Validation => StatusCodes.Status422UnprocessableEntity,
        RacksStandsResult.ResultState.Conflict => StatusCodes.Status409Conflict,
        RacksStandsResult.ResultState.Forbidden => StatusCodes.Status403Forbidden,
        RacksStandsResult.ResultState.Unauthorized => StatusCodes.Status401Unauthorized,
        RacksStandsResult.ResultState.Unavailable => StatusCodes.Status503ServiceUnavailable,
        RacksStandsResult.ResultState.Unexpected => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}
