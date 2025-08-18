using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using TMS.Services;

namespace TMS.Pages.Task
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

        public List<TaskDtos>? Tasks { get; set; }
        public UserDtos? CurrentUser { get; set; }
        public bool IsAdmin { get; set; }
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public string? PriorityFilter { get; set; }

        public async Task<IActionResult> OnGetAsync(string? search, string? status, string? priority)
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

                // Set filters
                SearchTerm = search;
                StatusFilter = status;
                PriorityFilter = priority;

                // Get tasks based on user role
                if (IsAdmin)
                {
                    Tasks = await _apiService.GetTasksAsync();
                }
                else
                {
                    Tasks = await _apiService.GetMyTasksAsync();
                }

                // Apply filters
                if (Tasks != null)
                {
                    Tasks = ApplyFilters(Tasks, search, status, priority);
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tasks");
                return RedirectToPage("/Error");
            }
        }

        private List<TaskDtos> ApplyFilters(List<TaskDtos> tasks, string? search, string? status, string? priority)
        {
            var filteredTasks = tasks.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrEmpty(search))
            {
                filteredTasks = filteredTasks.Where(t => 
                    t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    t.Description?.Contains(search, StringComparison.OrdinalIgnoreCase) == true ||
                    t.AssignedToUserName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
            }

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                filteredTasks = filteredTasks.Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            // Apply priority filter
            if (!string.IsNullOrEmpty(priority) && priority != "All")
            {
                filteredTasks = filteredTasks.Where(t => t.Priority.Equals(priority, StringComparison.OrdinalIgnoreCase));
            }

            return filteredTasks.ToList();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int taskId, string status)
        {
            try
            {
                var request = new UpdateTaskStatusRequest { Status = status };
                var updatedTask = await _apiService.UpdateTaskStatusAsync(taskId, request);
                
                if (updatedTask != null)
                {
                    TempData["SuccessMessage"] = "Task status updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update task status.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status");
                TempData["ErrorMessage"] = "An error occurred while updating task status.";
                return RedirectToPage();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int taskId)
        {
            try
            {
                if (!IsAdmin)
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete tasks.";
                    return RedirectToPage();
                }

                var success = await _apiService.DeleteTaskAsync(taskId);
                
                if (success)
                {
                    TempData["SuccessMessage"] = "Task deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete task.";
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                TempData["ErrorMessage"] = "An error occurred while deleting the task.";
                return RedirectToPage();
            }
        }
    }
}
