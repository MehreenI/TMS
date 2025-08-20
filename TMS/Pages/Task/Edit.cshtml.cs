using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text.Json;
using System.Text;

namespace TMS.Pages.Task
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        [BindProperty]
        public UpdateTaskRequest Task { get; set; } = new();

        public List<UserDtos> Users { get; set; } = new();
        public int TaskId { get; set; }

        public EditModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiUrl = configuration["ApiUrl"] ?? "http://localhost:5019";
        }

        public async System.Threading.Tasks.Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                var userRole = HttpContext.Session.GetString("UserRole");
                var isAdmin = !string.IsNullOrWhiteSpace(userRole) && userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                
                if (!isAdmin)
                {
                    return RedirectToPage("/Home/Index");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                TaskId = id;

                var taskEndpoint = $"{_apiUrl}/api/TaskItems/{id}";
                var taskResponse = await _httpClient.GetAsync(taskEndpoint);

                if (taskResponse.IsSuccessStatusCode)
                {
                    var taskContent = await taskResponse.Content.ReadAsStringAsync();
                    var task = JsonSerializer.Deserialize<TaskDtos>(taskContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (task != null)
                    {
                        Task = new UpdateTaskRequest
                        {
                            Title = task.Title,
                            Description = task.Description,
                            Deadline = task.Deadline,
                            Priority = task.Priority,
                            assignedUserId = task.AssignedUserId
                        };
                    }
                }

                // Load users for dropdown
                await LoadUsers();

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading task data.";
                return RedirectToPage("./Index");
            }
        }

        public async System.Threading.Tasks.Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadUsers();
                    return Page();
                }

                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                var userRole = HttpContext.Session.GetString("UserRole");
                var isAdmin = !string.IsNullOrWhiteSpace(userRole) && userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                
                if (!isAdmin)
                {
                    return RedirectToPage("/Home/Index");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiEndpoint = $"{_apiUrl}/api/TaskItems/{id}";
                var json = JsonSerializer.Serialize(Task);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Task updated successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    await LoadUsers();
                    return Page();
                }
            }
            catch (Exception ex)
            {
               
                await LoadUsers();
                return Page();
            }
        }

        private async System.Threading.Tasks.Task LoadUsers()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    var apiEndpoint = $"{_apiUrl}/api/Users";
                    var response = await _httpClient.GetAsync(apiEndpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var users = JsonSerializer.Deserialize<List<UserDtos>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (users != null)
                        {
                            Users = users;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users: {ex.Message}");
            }
        }
    }
}
