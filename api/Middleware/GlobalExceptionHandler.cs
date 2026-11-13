using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SavageMetals.Api.Core.Exceptions;

namespace SavageMetals.Api.Middleware;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            InvalidRequestException => (StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
            NotFoundException => (StatusCodes.Status404NotFound, "Not found", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
            SqlException { Number: 2601 or 2627 } => (StatusCodes.Status409Conflict, "Conflict", "A record with the same value already exists."),
            _ => (StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            }
        });
    }
}