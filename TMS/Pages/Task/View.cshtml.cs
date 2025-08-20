using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text.Json;

namespace TMS.Pages.Task
{
    public class ViewModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public TaskDtos Task { get; set; } = new();
        public UserDtos? AssignedUser { get; set; }
        public List<UserDtos> Users { get; set; } = new();

        public ViewModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
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

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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
                        Task = task;
                        return Page();
                    }
                }

                TempData["ErrorMessage"] = "Task not found.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading task data.";
                return RedirectToPage("./Index");
            }
        }
    }
}
