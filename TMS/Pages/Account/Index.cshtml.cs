using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.Account
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

        [BindProperty]
        public UpdateUserRequest UserRequest { get; set; } = new();

        [BindProperty]
        public ChangePasswordRequest PasswordRequest { get; set; } = new();

        public UserDtos? CurrentUser { get; set; }
        public List<SelectListItem> RoleOptions { get; set; } = new();
        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                CurrentUser = await _authService.GetCurrentUserAsync();
                if (CurrentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                IsAdmin = await _authService.IsAdminAsync();

                var parts = (CurrentUser.FullName ?? string.Empty).Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                UserRequest.FirstName = parts.Length > 0 ? parts[0] : string.Empty;
                UserRequest.LastName = parts.Length > 1 ? parts[1] : string.Empty;
                UserRequest.Email = CurrentUser.Email;
                UserRequest.PhoneNumber = CurrentUser.PhoneNumber ?? string.Empty;
                UserRequest.Role = CurrentUser.Role;

                if (IsAdmin)
                {
                    RoleOptions = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "User", Text = "User" },
                        new SelectListItem { Value = "Admin", Text = "Admin" }
                    };
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading account page");
                return RedirectToPage("/Error");
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            try
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                if (!ModelState.IsValid)
                {
                    await OnGetAsync();
                    return Page();
                }

                var updatedUser = await _apiService.UpdateUserAsync(currentUser.Id, UserRequest);
                
                if (updatedUser != null)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully.";
                    await OnGetAsync();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update profile.";
                    await OnGetAsync();
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile");
                TempData["ErrorMessage"] = "An error occurred while updating your profile.";
                await OnGetAsync();
                return Page();
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            try
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                if (!ModelState.IsValid)
                {
                    await OnGetAsync();
                    return Page();
                }

                if (PasswordRequest.NewPassword != PasswordRequest.ConfirmPassword)
                {
                    ModelState.AddModelError("PasswordRequest.ConfirmPassword", "New password and confirmation password do not match.");
                    await OnGetAsync();
                    return Page();
                }

                var success = await _apiService.ChangePasswordAsync(currentUser.Id, PasswordRequest);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Password changed successfully.";
                    PasswordRequest = new ChangePasswordRequest();
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to change password. Please check your current password.";
                }

                await OnGetAsync();
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                TempData["ErrorMessage"] = "An error occurred while changing your password.";
                await OnGetAsync();
                return Page();
            }
        }
    }
}
