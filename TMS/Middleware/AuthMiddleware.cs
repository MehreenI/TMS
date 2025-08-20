using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using TMS.Constants;

namespace TMS.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthMiddleware> _logger;

        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var path = context.Request.Path.Value?.ToLower();
                
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

                bool isPublicRoute = publicRoutes.Any(route => 
                    path?.StartsWith(route) == true || 
                    path?.Contains(route) == true);

                if (!isPublicRoute)
                {
                    var token = context.Session.GetString(AppConstants.SessionKeys.JwtToken);
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        _logger.LogInformation($"Unauthenticated access attempt to {path}");
                        context.Response.Redirect(AppConstants.Routes.Login);
                        return;
                    }

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
                        var userRole = context.Session.GetString(AppConstants.SessionKeys.UserRole);
                        var isAdmin = !string.IsNullOrWhiteSpace(userRole) && userRole.Equals(AppConstants.Roles.Admin, StringComparison.OrdinalIgnoreCase);
                        
                        if (!isAdmin)
                        {
                            _logger.LogWarning($"Non-admin user attempted to access admin route: {path}");
                            context.Response.Redirect(AppConstants.Routes.Error);
                            return;
                        }
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AuthMiddleware");
                context.Response.Redirect(AppConstants.Routes.Error);
                return;
            }
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
