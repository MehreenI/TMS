using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Services;

namespace TMS.Pages.Home
{
    public class LogoutModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(IAuthService authService, ILogger<LogoutModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                await _authService.LogoutAsync();
                _logger.LogInformation("User logged out successfully");
                return RedirectToPage("/Home/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return RedirectToPage("/Home/Login");
            }
        }
    }
}
