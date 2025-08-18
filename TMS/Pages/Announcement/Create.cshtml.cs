using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.Announcement
{
	public class CreateModel : PageModel
	{
		private readonly IAuthService _authService;
		private readonly IApiService _apiService;
		private readonly ILogger<CreateModel> _logger;

		public CreateModel(IAuthService authService, IApiService apiService, ILogger<CreateModel> logger)
		{
			_authService = authService;
			_apiService = apiService;
			_logger = logger;
		}

		[BindProperty]
		public CreateAnnouncementRequest Announcement { get; set; } = new();

		public async Task<IActionResult> OnGetAsync()
		{
			var isAdmin = await _authService.IsAdminAsync();
			if (!isAdmin) return Unauthorized();
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			try
			{
				var isAdmin = await _authService.IsAdminAsync();
				if (!isAdmin) return Unauthorized();
				if (!ModelState.IsValid) return Page();
				var created = await _apiService.CreateAnnouncementAsync(Announcement);
				if (created != null)
				{
					TempData["SuccessMessage"] = "Announcement created.";
					return RedirectToPage("/Announcement/Index");
				}
				TempData["ErrorMessage"] = "Failed to create announcement.";
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating announcement");
				TempData["ErrorMessage"] = "An error occurred while creating the announcement.";
				return Page();
			}
		}
	}
}
