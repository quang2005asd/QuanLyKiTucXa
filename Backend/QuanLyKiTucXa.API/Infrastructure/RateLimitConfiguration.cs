namespace QuanLyKiTucXa.API.Infrastructure;

/// <summary>
/// Rate limiting configuration - reserved for future implementation
/// AspNetCoreRateLimit package has compatibility issues with current setup.
/// To be replaced with custom middleware approach.
/// </summary>
public static class RateLimitServiceConfiguration
{
    /// <summary>
    /// Placeholder - rate limiting will be implemented with custom middleware
    /// </summary>
    public static void ConfigureRateLimit(this IServiceCollection services)
    {
        // Placeholder - to be implemented with custom middleware
    }

    public static void UseRateLimit(this IApplicationBuilder app)
    {
        // Placeholder - to be implemented with custom middleware
    }
}
