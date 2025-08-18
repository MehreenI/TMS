using TMS.Models.ViewModels;

namespace TMS.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task<bool> ValidateTokenAsync(string token);
        Task LogoutAsync();
        Task<UserDtos?> GetCurrentUserAsync();
        Task<string?> GetTokenAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<bool> IsAdminAsync();
        Task<bool> IsUserAsync();
        Task<int?> GetCurrentUserIdAsync();
    }
}
