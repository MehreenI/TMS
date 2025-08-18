using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.Task
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
        public CreateTaskRequest TaskRequest { get; set; } = new();

        public List<UserDtos>? AvailableUsers { get; set; }
        public List<SelectListItem> PriorityOptions { get; set; } = new();
        public UserDtos? CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check if user is admin
                var isAdmin = await _authService.IsAdminAsync();
                if (!isAdmin)
                {
                    TempData["ErrorMessage"] = "You don't have permission to create tasks.";
                    return RedirectToPage("/Task/Index");
                }

                // Get current user
                CurrentUser = await _authService.GetCurrentUserAsync();
                if (CurrentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                // Get available users for assignment
                AvailableUsers = await _apiService.GetUsersAsync();

                // Set up priority options
                PriorityOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Low", Text = "Low" },
                    new SelectListItem { Value = "Medium", Text = "Medium", Selected = true },
                    new SelectListItem { Value = "High", Text = "High" }
                };

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create task page");
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
                    TempData["ErrorMessage"] = "You don't have permission to create tasks.";
                    return RedirectToPage("/Task/Index");
                }

                if (!ModelState.IsValid)
                {
                    // Reload the page with validation errors
                    await OnGetAsync();
                    return Page();
                }

                // Get current user for CreatedByUserId
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return RedirectToPage("/Home/Login");
                }

                // Create the task
                var createdTask = await _apiService.CreateTaskAsync(TaskRequest);
                
                if (createdTask != null)
                {
                    TempData["SuccessMessage"] = "Task created successfully.";
                    return RedirectToPage("/Task/Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create task.";
                    await OnGetAsync();
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                TempData["ErrorMessage"] = "An error occurred while creating the task.";
                await OnGetAsync();
                return Page();
            }
        }
    }
}
