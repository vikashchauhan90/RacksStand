using Hal.Core;
using RacksStands.Framework.Base.Serializers;
using ResultifyCore.Exceptions;
using System.Diagnostics;

namespace RacksStands.ApiHost.Middlewares;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An Error occured.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private  Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            return Task.CompletedTask;
        }


        var acceptHeader = context.Request.Headers.Accept.ToString();
        var contentType = string.IsNullOrWhiteSpace(acceptHeader)
            ? ContentType.Json
            : ContentTypes.Resolve(acceptHeader);

        var (statusCode, apiResponse) = BuildApiResponse(context, exception);
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = contentType switch
        {
            ContentType.Json => ContentTypes.ApplicationJson,
            ContentType.Xml => ContentTypes.ApplicationXml,
            _ => ContentTypes.TextPlain
        };

        return contentType switch
        {
            ContentType.Json => context.Response.WriteAsJsonAsync(apiResponse, JsonSerializerHelper.Options),
            ContentType.Xml => context.Response.WriteAsync(XmlSerializerHelper.SerializeString(apiResponse)),
            _ => context.Response.WriteAsync(TextSerializerHelper.SerializeString(apiResponse))
        };

    }

    private static (int statusCode, ApiResponse) BuildApiResponse(
        HttpContext context,
        Exception exception)
    {
        var trackingId = context.TraceIdentifier ?? Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString();
        var correlationId = context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationIdHeader) ? correlationIdHeader.ToString() : null;

        var meta = new Dictionary<string, object>(StringComparer.Ordinal);
        if (!string.IsNullOrEmpty(correlationId))
        {
            meta["CorrelationId"] = correlationId;
        }
        meta["TrackingId"] = trackingId;

        return exception switch
        {
            ResultifyBaseException ex =>
            (
                ex.StatusCode,
                new ApiResponse
                {
                    Success = false,
                    Error = ex.Message,
                    ErrorMeta = MergeFieldErrors(meta, ex.Errors)
                }
            ),
            _ =>
            (
                StatusCodes.Status500InternalServerError,
                new ApiResponse
                {
                    Success = false,
                    Error = "An unexpected error occurred.",
                    ErrorMeta = meta
                }
            )
        };
    }

    private static IReadOnlyDictionary<string, object> MergeFieldErrors(
        Dictionary<string, object> meta,
        IReadOnlyDictionary<string, object> fieldErrors)
    {
        foreach (var error in fieldErrors)
        {
            meta[error.Key] = error.Value;
        }
        return meta;
    }
}
