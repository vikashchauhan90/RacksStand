using Microsoft.AspNetCore.Http;
using RacksStands.Framework.Base.Serializers;
using HttpResults = Microsoft.AspNetCore.Http.Results;
using Hal.Core;

namespace RacksStands.Framework.Modules.Bootstrap;

public static class HttpResultExtensions
{
    public static IResult ToHttpResult(this ResultifyCore.IResult domainResult, HttpContext httpContext)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        if (domainResult.Status == ResultifyCore.ResultState.NoContent)
        {
            return TypedResults.NoContent();
        }


        if (domainResult.Status == ResultifyCore.ResultState.Success)
        {
            return TypedResults.Ok();
        }

        if (domainResult.Status == ResultifyCore.ResultState.Created)
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
    public static IResult ToHttpResult<T>(this ResultifyCore.IResult<T> domainResult, HttpContext httpContext)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        // Success branch
        if (domainResult.Status == ResultifyCore.ResultState.Success ||
            domainResult.Status == ResultifyCore.ResultState.Created ||
            domainResult.Status == ResultifyCore.ResultState.NoContent)
        {
            if (domainResult.Status == ResultifyCore.ResultState.NoContent)
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
    public static IResult ToApiResult(this ResultifyCore.IResult domainResult, HttpContext httpContext, string? errorMessage = null)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        if (domainResult.Status == ResultifyCore.ResultState.NoContent)
        {
            return TypedResults.NoContent();
        }


        if (domainResult.Status == ResultifyCore.ResultState.Success)
        {
            return TypedResults.Ok(new ApiResponse { Success = true });
        }

        if (domainResult.Status == ResultifyCore.ResultState.Created)
        {
            return TypedResults.Created();
        }

        var errorSerialized = ResponseSerializer.Serialize(new ApiResponse
        {
            Success = false,
            Error = errorMessage ?? string.Empty,
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
    public static IResult ToApiResult<T>(this ResultifyCore.IResult<T> domainResult, HttpContext httpContext, string? errorMessage = null)
    {
        var acceptHeader = httpContext.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        // Success branch
        if (domainResult.Status == ResultifyCore.ResultState.Success ||
            domainResult.Status == ResultifyCore.ResultState.Created ||
            domainResult.Status == ResultifyCore.ResultState.NoContent)
        {
            if (domainResult.Status == ResultifyCore.ResultState.NoContent)
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
            Error = errorMessage ?? string.Empty,
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
    private static int MapStatusToHttpCode(ResultifyCore.ResultState state) => state switch
    {
        ResultifyCore.ResultState.Success => StatusCodes.Status200OK,
        ResultifyCore.ResultState.Created => StatusCodes.Status201Created,
        ResultifyCore.ResultState.NoContent => StatusCodes.Status204NoContent,
        ResultifyCore.ResultState.NotFound => StatusCodes.Status404NotFound,
        ResultifyCore.ResultState.Validation => StatusCodes.Status422UnprocessableEntity,
        ResultifyCore.ResultState.Conflict => StatusCodes.Status409Conflict,
        ResultifyCore.ResultState.Forbidden => StatusCodes.Status403Forbidden,
        ResultifyCore.ResultState.Unauthorized => StatusCodes.Status401Unauthorized,
        ResultifyCore.ResultState.Unavailable => StatusCodes.Status503ServiceUnavailable,
        ResultifyCore.ResultState.Unexpected => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError
    };
}
