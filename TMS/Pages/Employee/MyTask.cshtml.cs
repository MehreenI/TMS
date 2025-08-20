using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models;
using TMS.Models.ViewModels;

namespace TMS.Pages.Employee
{
    public class MyTaskModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;
        private readonly ILogger<MyTaskModel> _logger;

        public List<TaskDtos> Tasks { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;

        public MyTaskModel(HttpClient httpClient, IConfiguration configuration, ILogger<MyTaskModel> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _apiUrl = configuration["ApiUrl"] ?? "http://localhost:5019";
        }

        public async System.Threading.Tasks.Task<IActionResult> OnGetAsync()
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

                var apiEndpoint = $"{_apiUrl}/api/TaskItems/my-tasks";
                var response = await _httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        var tasks = JsonSerializer.Deserialize<List<TaskDtos>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        if (tasks != null)
                        {
                            Tasks = tasks;
                        }
                    }

                    return Page();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    HttpContext.Session.Remove("JWTToken");
                    return RedirectToPage("/Home/Login");
                }
                else
                {
                    _logger.LogError("Failed to fetch tasks. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, await response.Content.ReadAsStringAsync());

                    ErrorMessage = "Unable to load tasks at this time. Please try again later.";
                    return Page();
                }
            }
           
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching tasks");
                ErrorMessage = "An unexpected error occurred. Please try again later.";
                return Page();
            }
        }
    }
}