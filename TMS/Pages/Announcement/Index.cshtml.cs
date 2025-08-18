using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Services;

namespace TMS.Pages.Announcement
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

		public List<TMS.Models.AnnouncementData>? Announcements { get; set; }
		public bool IsAdmin { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			try
			{
				IsAdmin = await _authService.IsAdminAsync();
				Announcements = await _apiService.GetAnnouncementsAsync();
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading announcements");
				return RedirectToPage("/Error");
			}
		}

		public async Task<IActionResult> OnPostDeleteAsync(int id)
		{
			try
			{
				var isAdmin = await _authService.IsAdminAsync();
				if (!isAdmin) return Unauthorized();
				var success = await _apiService.DeleteAnnouncementAsync(id);
				TempData[success ? "SuccessMessage" : "ErrorMessage"] = success ? "Announcement deleted." : "Failed to delete announcement.";
				return RedirectToPage();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting announcement");
				TempData["ErrorMessage"] = "An error occurred while deleting the announcement.";
				return RedirectToPage();
			}
		}
	}
}
