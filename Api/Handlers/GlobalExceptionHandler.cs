using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Handlers;

/// <summary>
/// global handler that intercepts unhandled exceptions and converts them to standardized problem details responses
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // capture the unique trace identifier for this request to aid in debugging
        var traceId = httpContext.TraceIdentifier;

        // log the error including the trace id for correlation
        logger.LogError(
            exception, 
            "an unhandled exception has occurred. traceid: {TraceId}, message: {Message}", 
            traceId, 
            exception.Message
        );

        // determine status code and title based on exception type using pattern matching
        var (statusCode, title, detail) = MapException(exception);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}", // standard rfc 7807 type
            Extensions =
            {
                ["traceId"] = traceId 
            }
        };

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // signals that the exception was handled successfully
        return true; 
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            // handles tenant resolution specific errors
            InvalidOperationException ex when ex.Message.Contains("Tenant") 
                => (StatusCodes.Status400BadRequest, "Tenant Resolution Error", ex.Message),
            // handles domain validation errors
            ArgumentException ex 
                => (StatusCodes.Status400BadRequest, "Invalid Argument", ex.Message),
            // handles resources not found
            KeyNotFoundException ex 
                => (StatusCodes.Status404NotFound, "Resource Not Found", ex.Message),
            // handles unauthorized access attempts
            UnauthorizedAccessException ex
                => (StatusCodes.Status401Unauthorized, "Unauthorized", "You do not have permission to access this resource."),
            // default fallback for unexpected errors
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };
    }
}