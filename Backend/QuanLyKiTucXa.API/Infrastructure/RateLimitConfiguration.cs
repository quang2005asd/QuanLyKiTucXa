using AspNetCoreRateLimit;

namespace QuanLyKiTucXa.API.Infrastructure;

public static class RateLimitServiceConfiguration
{
    public static void ConfigureRateLimit(this IServiceCollection services)
    {
        // Load rate limit configuration from appsettings
        services.AddMemoryCache();
        services.AddInMemoryRateLimiting();

        // Add custom rate limit policy
        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429; // Too Many Requests
            options.RealIpHeader = "X-Real-IP";
            options.ClientIdHeader = "X-ClientId";
            options.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1m", // 1 minute
                    Limit = 100    // 100 requests per minute
                },
                new RateLimitRule
                {
                    Endpoint = "*/auth/*",
                    Period = "5m", // 5 minutes
                    Limit = 10     // 10 auth attempts per 5 minutes
                },
                new RateLimitRule
                {
                    Endpoint = "*/students/*",
                    Period = "1m",
                    Limit = 50
                }
            };
        });

        services.Configure<IpRateLimitPolicies>(options =>
        {
            options.IpRules = new List<IpRateLimitPolicy>();
        });
    }

    public static void UseRateLimit(this IApplicationBuilder app)
    {
        app.UseIpRateLimiting();
    }
}
