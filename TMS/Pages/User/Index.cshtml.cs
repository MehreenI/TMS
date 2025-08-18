using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.User
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

        public List<UserDtos>? Users { get; set; }
        public UserDtos? CurrentUser { get; set; }
        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(string? search, string? role)
        {
            try
            {
                // Check if user is admin
                var isAdmin = await _authService.IsAdminAsync();
                if (!isAdmin)
                {
                    TempData["ErrorMessage"] = "You don't have permission to access user management.";
                    return RedirectToPage("/Home/Index");
                }

                // Get current user
                CurrentUser = await _authService.GetCurrentUserAsync();
                if (CurrentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                // Set filters
                SearchTerm = search;
                RoleFilter = role;

                // Get all users
                Users = await _apiService.GetUsersAsync();

                // Apply filters
                if (Users != null)
                {
                    Users = ApplyFilters(Users, search, role);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users");
                return RedirectToPage("/Error");
            }
        }

        private List<UserDtos> ApplyFilters(List<UserDtos> users, string? search, string? role)
        {
            var filteredUsers = users.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                filteredUsers = filteredUsers.Where(u => 
                    u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // Apply role filter
            if (!string.IsNullOrEmpty(role) && role != "All")
            {
                filteredUsers = filteredUsers.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase));
            }

            return filteredUsers.ToList();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            try
            {
                // Check if user is admin
                var isAdmin = await _authService.IsAdminAsync();
                if (!isAdmin)
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete users.";
                    return RedirectToPage();
                }

                // Get current user to prevent self-deletion
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser?.Id == userId)
                {
                    TempData["ErrorMessage"] = "You cannot delete your own account.";
                    return RedirectToPage();
                }

                var success = await _apiService.DeleteUserAsync(userId);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "User deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete user.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                TempData["ErrorMessage"] = "An error occurred while deleting the user.";
                return RedirectToPage();
            }
        }
    }
}
