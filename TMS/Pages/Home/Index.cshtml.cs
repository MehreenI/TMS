using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using TMS.Models;
using TMS.Services;

namespace TMS.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly IApiService _apiService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IAuthService authService, IApiService apiService, ILogger<IndexModel> logger)
        {
            _authService = authService;
            _apiService = apiService;
            _logger = logger;
        }

        public UserDtos? CurrentUser { get; set; }
        public DashboardStats? DashboardStats { get; set; }
        public List<TaskDtos>? MyTasks { get; set; }
        public List<TaskDtos>? DueSoonTasks { get; set; }
        public List<AnnouncementData>? Announcements { get; set; }
        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Get current user
                CurrentUser = await _authService.GetCurrentUserAsync();
                if (CurrentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                // Check if user is admin
                IsAdmin = await _authService.IsAdminAsync();

                // Get dashboard statistics
                DashboardStats = await _apiService.GetDashboardStatsAsync();

                // Get user's tasks
                MyTasks = await _apiService.GetMyTasksAsync();

                // Get due soon tasks
                DueSoonTasks = await _apiService.GetDueSoonTasksAsync(24);

                // Get announcements
                Announcements = await _apiService.GetAnnouncementsAsync();

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                return RedirectToPage("/Error");
            }
        }
    }
}
