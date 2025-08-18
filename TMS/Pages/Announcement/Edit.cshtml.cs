using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.Announcement
{
	public class EditModel : PageModel
	{
		private readonly IAuthService _authService;
		private readonly IApiService _apiService;
		private readonly ILogger<EditModel> _logger;

		public EditModel(IAuthService authService, IApiService apiService, ILogger<EditModel> logger)
		{
			_authService = authService;
			_apiService = apiService;
			_logger = logger;
		}

		[BindProperty]
		public UpdateAnnouncementRequest Announcement { get; set; } = new();
		public int Id { get; set; }

		public async Task<IActionResult> OnGetAsync(int id)
		{
			try
			{
				var isAdmin = await _authService.IsAdminAsync();
				if (!isAdmin) return Unauthorized();
				var ann = await _apiService.GetAnnouncementAsync(id);
				if (ann == null) return RedirectToPage("/Announcement/Index");
				Id = id;
				Announcement.Title = ann.Title;
				Announcement.Content = ann.Content;
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error loading announcement");
				return RedirectToPage("/Announcement/Index");
			}
		}

		public async Task<IActionResult> OnPostAsync(int id)
		{
			try
			{
				var isAdmin = await _authService.IsAdminAsync();
				if (!isAdmin) return Unauthorized();
				if (!ModelState.IsValid) return Page();
				var updated = await _apiService.UpdateAnnouncementAsync(id, Announcement);
				if (updated != null)
				{
					TempData["SuccessMessage"] = "Announcement updated.";
					return RedirectToPage("/Announcement/Index");
				}
				TempData["ErrorMessage"] = "Failed to update announcement.";
				return Page();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating announcement");
				TempData["ErrorMessage"] = "An error occurred while updating the announcement.";
				return Page();
			}
		}
	}
}
