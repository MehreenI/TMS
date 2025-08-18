using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using TMS.Models.ViewModels;

namespace TMS.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IApiService _apiService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IHttpContextAccessor httpContextAccessor, IApiService apiService, ILogger<AuthService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _apiService = apiService;
            _logger = logger;
        }

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                var response = await _apiService.LoginAsync(loginRequest);
                
                if (response?.Success == true && !string.IsNullOrEmpty(response.Token))
                {
                    var session = _httpContextAccessor.HttpContext?.Session;
                    if (session != null)
                    {
                        session.SetString("JWTToken", response.Token);
                        session.SetString("User", JsonSerializer.Serialize(response.User));
                        session.SetString("RefreshToken", response.RefreshToken ?? "");
                        session.SetString("ExpiresAt", response.ExpiresAt?.ToString() ?? "");
                        
                        return true;
                    }
                }
                
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return false;
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                return await _apiService.ValidateTokenAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }

        public Task LogoutAsync()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    session.Clear();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }

            return Task.CompletedTask;
        }

        public Task<UserDtos?> GetCurrentUserAsync()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    var userJson = session.GetString("User");
                    if (!string.IsNullOrEmpty(userJson))
                    {
                        var user = JsonSerializer.Deserialize<UserDtos>(userJson);
                        return Task.FromResult<UserDtos?>(user);
                    }
                }

                return Task.FromResult<UserDtos?>(null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return Task.FromResult<UserDtos?>(null);
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    var token = session.GetString("JWTToken");
                    if (!string.IsNullOrEmpty(token))
                    {
                        // Check if token is expired
                        var expiresAtStr = session.GetString("ExpiresAt");
                        if (!string.IsNullOrEmpty(expiresAtStr) && DateTime.TryParse(expiresAtStr, out var expiresAt))
                        {
                            if (DateTime.UtcNow < expiresAt)
                            {
                                return token;
                            }
                            else
                            {
                                // Token expired, clear session
                                await LogoutAsync();
                                return null;
                            }
                        }
                        return token;
                    }
                }
                
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting token");
                return null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return false;

                return await ValidateTokenAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking authentication");
                return false;
            }
        }

        public async Task<bool> IsAdminAsync()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                return user?.Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking admin role");
                return false;
            }
        }

        public async Task<bool> IsUserAsync()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                return user?.Role?.Equals("User", StringComparison.OrdinalIgnoreCase) == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking user role");
                return false;
            }
        }

        public async Task<int?> GetCurrentUserIdAsync()
        {
            try
            {
                var user = await GetCurrentUserAsync();
                return user?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user ID");
                return null;
            }
        }
    }
}
