using System.Text.RegularExpressions;

namespace QuanLyKiTucXa.API.Infrastructure;

/// <summary>
/// Input sanitization middleware to prevent XSS and injection attacks
/// </summary>
public class InputSanitizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<InputSanitizationMiddleware> _logger;

    public InputSanitizationMiddleware(RequestDelegate next, ILogger<InputSanitizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Sanitize query string parameters
        if (context.Request.QueryString.HasValue)
        {
            var queryString = context.Request.QueryString.Value;
            if (ContainsSuspiciousContent(queryString))
            {
                _logger.LogWarning("Suspicious query string detected: {QueryString}", queryString);
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid input detected");
                return;
            }
        }

        // Sanitize request headers
        if (context.Request.Headers.ContainsKey("User-Agent"))
        {
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            if (userAgent.Length > 500)
            {
                _logger.LogWarning("Unusually long User-Agent header detected");
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid header");
                return;
            }
        }

        await _next(context);
    }

    private static bool ContainsSuspiciousContent(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        // Check for common SQL injection patterns
        var sqlPatterns = new[] { "' OR '", "'; DROP", "UNION SELECT", "--", "/*", "*/" };
        if (sqlPatterns.Any(pattern => input.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            return true;

        // Check for XSS patterns
        var xssPatterns = new[] { "<script", "javascript:", "onerror=", "onload=" };
        if (xssPatterns.Any(pattern => input.Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            return true;

        return false;
    }
}

public static class InputSanitizationMiddlewareExtensions
{
    public static IApplicationBuilder UseInputSanitization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<InputSanitizationMiddleware>();
    }
}
