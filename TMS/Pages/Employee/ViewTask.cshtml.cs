using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;

namespace TMS.Pages.Employee
{
    public class ViewTaskModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;
        private readonly ILogger<ViewTaskModel> _logger;

        [BindProperty]
        public TaskDtos Task { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;

        public ViewTaskModel(HttpClient httpClient, IConfiguration configuration, ILogger<ViewTaskModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiUrl = configuration["ApiUrl"] ?? "http://localhost:5019";
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var taskEndpoint = $"{_apiUrl}/api/TaskItems/{id}";
                var taskResponse = await _httpClient.GetAsync(taskEndpoint);

                if (taskResponse.IsSuccessStatusCode)
                {
                    var taskContent = await taskResponse.Content.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(taskContent))
                    {
                        var task = JsonSerializer.Deserialize<TaskDtos>(taskContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (task != null)
                        {
                            Task = task; 
                            return Page();
                        }
                    }
                }
                else if (taskResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToPage("/Home/Login");
                }
                else if (taskResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Task not found.";
                    return RedirectToPage("./MyTask");
                }

                _logger.LogError("Failed to fetch task details. Status: {StatusCode}", taskResponse.StatusCode);
                TempData["ErrorMessage"] = "Error loading task data.";
                return RedirectToPage("./MyTask");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching task details for ID: {TaskId}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return RedirectToPage("./MyTask");
            }
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(string newStatus)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                if (string.IsNullOrWhiteSpace(newStatus) ||
                    !new[] { "ToDo", "InProgress", "Done" }.Contains(newStatus))
                {
                    TempData["ErrorMessage"] = "Invalid status value.";
                    return Page();
                }

                var updateData = new
                {
                    Id = Task.Id,
                    Status = newStatus
                };

                var json = JsonSerializer.Serialize(updateData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var apiEndpoint = $"{_apiUrl}/api/TaskItems/{Task.Id}/status";
                var response = await _httpClient.PatchAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Task.Status = newStatus; 
                    TempData["SuccessMessage"] = "Task status updated successfully!";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToPage("/Home/Login");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to update task status. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, errorContent);
                    TempData["ErrorMessage"] = "Failed to update task status. Please try again.";
                }
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating task status for ID: {TaskId}", Task?.Id);
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
            }

            return Page();
        }
    }
}