using System.Net;
using System.Text.Json;
using QuanLyKiTucXa.API.DTOs;

namespace QuanLyKiTucXa.API.Infrastructure;

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
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var response = new ApiResponseDto<object>
        {
            Success = false,
            Message = "An internal server error occurred. Please try again later.",
            Data = null,
            Errors = null
        };

        // Log detailed error information (in production, log to external service)
        if (exception is ArgumentException or ArgumentNullException or InvalidOperationException)
        {
            response.Message = exception.Message;
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
        else if (exception is UnauthorizedAccessException)
        {
            response.Message = "Unauthorized access";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
        else if (exception is KeyNotFoundException)
        {
            response.Message = "Resource not found";
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
