using Microsoft.AspNetCore.Http.Extensions;
using TMS.Services;

namespace TMS.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            var path = context.Request.Path.Value?.ToLower();
            
            // Public routes that don't require authentication
            var publicRoutes = new[]
            {
                "/home/login",
                "/home/index",
                "/error",
                "/css/",
                "/js/",
                "/lib/",
                "/images/",
                "/favicon.ico"
            };

            // Check if the current path is a public route
            bool isPublicRoute = publicRoutes.Any(route => 
                path?.StartsWith(route) == true || 
                path?.Contains(route) == true);

            if (!isPublicRoute)
            {
                // Check if user is authenticated
                var isAuthenticated = await authService.IsAuthenticatedAsync();
                
                if (!isAuthenticated)
                {
                    _logger.LogWarning("Unauthenticated access attempt to: {Path}", path);
                    context.Response.Redirect("/Home/Login");
                    return;
                }

                // Check for admin-only routes
                var adminRoutes = new[]
                {
                    "/user/",
                    "/task/create",
                    "/task/edit",
                    "/task/delete",
                    "/dashboard/analytics",
                    "/dashboard/report",
                    "/announcement/",
                    "/email/"
                };

                bool isAdminRoute = adminRoutes.Any(route => 
                    path?.StartsWith(route) == true);

                if (isAdminRoute)
                {
                    var isAdmin = await authService.IsAdminAsync();
                    if (!isAdmin)
                    {
                        _logger.LogWarning("Unauthorized admin access attempt to: {Path}", path);
                        context.Response.Redirect("/Home/Index");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

    // Extension method for easy middleware registration
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
