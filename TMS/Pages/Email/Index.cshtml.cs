using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.Email
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

		public async Task<IActionResult> OnGetAsync()
		{
			var isAdmin = await _authService.IsAdminAsync();
			if (!isAdmin)
			{
				TempData["ErrorMessage"] = "You don't have permission to manage email templates.";
				return RedirectToPage("/Home/Index");
			}
			return Page();
		}

		[ValidateAntiForgeryToken]
		public async Task<IActionResult> OnPostSaveTemplateAsync(string templateId, string subject, string content)
		{
			try
			{
				var isAdmin = await _authService.IsAdminAsync();
				if (!isAdmin)
				{
					return Unauthorized();
				}

				var request = new EmailTemplateUpdateRequest { Subject = subject, Content = content };
				var success = await _apiService.UpdateEmailTemplateAsync(templateId, request);

				if (success)
				{
					TempData["SuccessMessage"] = "Template saved successfully.";
				}
				else
				{
					TempData["ErrorMessage"] = "Failed to save template.";
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error saving email template");
				TempData["ErrorMessage"] = "An error occurred while saving the template.";
			}

			return RedirectToPage();
		}
	}
}
