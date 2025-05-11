using Casbin;

public class CasbinAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IEnforcer _enforcer;

    public CasbinAuthorizationMiddleware(RequestDelegate next, IEnforcer enforcer)
    {
        _next = next;
        _enforcer = enforcer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;

        // Skip authorization if user is not authenticated
        if (!user.Identity.IsAuthenticated)
        {
            await _next(context);
            return;
        }

        // Extract tenant claim
        var tenantClaim = user.FindFirst("tenant");
        var tenant = tenantClaim?.Value ?? "default";

        // Extract roles
        var roles = user.FindAll("role").Select(c => c.Value).ToList();
        if (!roles.Any())
        {
            roles.Add("anonymous"); // Fallback role
        }

        // Get request details
        var path = context.Request.Path.Value;
        var method = context.Request.Method;

        // Check permissions for each role
        bool hasPermission = false;
        foreach (var role in roles)
        {
            if (_enforcer.Enforce(role, tenant, path, method))
            {
                hasPermission = true;
                break;
            }
        }

        if (hasPermission)
        {
            await _next(context);
        }
        else
        {
            // Access denied
            context.Response.StatusCode = 403; // Forbidden
            await context.Response.WriteAsync("Access denied");
        }
    }
}

// Extension method for easy middleware registration
public static class CasbinMiddlewareExtensions
{
    public static IApplicationBuilder UseCasbinAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CasbinAuthorizationMiddleware>();
    }
}