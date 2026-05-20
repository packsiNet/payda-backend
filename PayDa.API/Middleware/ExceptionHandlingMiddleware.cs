using System.Net;
using System.Text.Json;
using PayDa.Application.Common.Exceptions;

namespace PayDa.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            ValidationException vex => (HttpStatusCode.UnprocessableEntity, vex.Message, vex.Errors),
            NotFoundException => (HttpStatusCode.NotFound, exception.Message, (IDictionary<string, string[]>?)null),
            ForbiddenException => (HttpStatusCode.Forbidden, exception.Message, null),
            UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message, null),
            BadRequestException => (HttpStatusCode.BadRequest, exception.Message, null),
            _ => (HttpStatusCode.InternalServerError, exception.Message, null)
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new { message, errors, stackTrace = exception.StackTrace };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
