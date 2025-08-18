using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.User
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
        public CreateUserRequest UserRequest { get; set; } = new();

        public List<SelectListItem> RoleOptions { get; set; } = new();
        public UserDtos? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check if user is admin
                var isAdmin = await _authService.IsAdminAsync();
                if (!isAdmin)
                {
                    TempData["ErrorMessage"] = "You don't have permission to create users.";
                    return RedirectToPage("/Home/Index");
                }

                // Get current user
                CurrentUser = await _authService.GetCurrentUserAsync();
                if (CurrentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                // Set up role options
                RoleOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "User", Text = "User", Selected = true },
                    new SelectListItem { Value = "Admin", Text = "Admin" }
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create user page");
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                // Check if user is admin
                var isAdmin = await _authService.IsAdminAsync();
                if (!isAdmin)
                {
                    TempData["ErrorMessage"] = "You don't have permission to create users.";
                    return RedirectToPage("/Home/Index");
                }

                if (!ModelState.IsValid)
                {
                    // Reload the page with validation errors
                    await OnGetAsync();
                    return Page();
                }

                // Create the user
                var createdUser = await _apiService.CreateUserAsync(UserRequest);
                
                if (createdUser != null)
                {
                    TempData["SuccessMessage"] = "User created successfully. A default password has been generated.";
                    return RedirectToPage("/User/Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create user.";
                    await OnGetAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                TempData["ErrorMessage"] = "An error occurred while creating the user.";
                await OnGetAsync();
                return Page();
            }
        }
    }
}
