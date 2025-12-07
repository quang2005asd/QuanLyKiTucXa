namespace QuanLyKiTucXa.API.Infrastructure;

/// <summary>
/// Role constants for the application
/// </summary>
public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Staff = "Staff";
    public const string Student = "Student";

    public static readonly List<string> AllRoles = new() { Admin, Manager, Staff, Student };

    public static bool IsValidRole(string role)
    {
        return AllRoles.Contains(role);
    }
}

/// <summary>
/// Configure authorization policies
/// </summary>
public static class AuthorizationPolicyConfiguration
{
    public static void ConfigureAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Admin policy - full access
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole(UserRoles.Admin));

            // Manager policy - can manage users and rooms
            options.AddPolicy("ManagerOrAdmin", policy =>
                policy.RequireRole(UserRoles.Admin, UserRoles.Manager));

            // Staff policy - can view data
            options.AddPolicy("StaffOrAbove", policy =>
                policy.RequireRole(UserRoles.Admin, UserRoles.Manager, UserRoles.Staff));

            // Student policy - view own data
            options.AddPolicy("StudentOrAbove", policy =>
                policy.RequireRole(UserRoles.Admin, UserRoles.Manager, UserRoles.Staff, UserRoles.Student));

            // Active users only
            options.AddPolicy("ActiveUserOnly", policy =>
                policy.RequireAssertion(context =>
                {
                    var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    return userIdClaim != null;
                }));
        });
    }
}
