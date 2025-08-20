using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text.Json;
using System.Web;

namespace TMS.Pages.Task
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public List<TaskDtos> Tasks { get; set; } = new();
        public List<UserDtos> Users { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Priority { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? AssignedUserId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? Deadline { get; set; }

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
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

                await LoadUsersAsync();

                var apiEndpoint = BuildApiEndpoint();

                var response = await _httpClient.GetAsync(apiEndpoint);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tasks = JsonSerializer.Deserialize<List<TaskDtos>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (tasks != null)
                    {
                        Tasks = tasks;

                        if (!string.IsNullOrWhiteSpace(SearchTerm))
                        {
                            Tasks = Tasks.Where(t =>
                                (t.Title?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (t.Description?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                            ).ToList();
                        }
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to load tasks.";
                return Page();
            }
        }

        private async System.Threading.Tasks.Task LoadUsersAsync()
        {
            try
            {
                var usersEndpoint = $"{_apiUrl}/api/Users";
                var response = await _httpClient.GetAsync(usersEndpoint);
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
            catch (Exception)
            {
                Users = new List<UserDtos>();
            }
        }

        private string BuildApiEndpoint()
        {
            var baseEndpoint = $"{_apiUrl}/api/TaskItems";
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(Priority))
            {
                queryParams.Add($"priority={HttpUtility.UrlEncode(Priority)}");
            }

            if (!string.IsNullOrWhiteSpace(Status))
            {
                queryParams.Add($"status={HttpUtility.UrlEncode(Status)}");
            }

            if (!string.IsNullOrWhiteSpace(AssignedUserId))
            {
                queryParams.Add($"assignedUserId={HttpUtility.UrlEncode(AssignedUserId)}");
            }

            if (Deadline.HasValue)
            {
                queryParams.Add($"deadline={Deadline.Value:yyyy-MM-dd}");
            }

            if (queryParams.Any())
            {
                return $"{baseEndpoint}?{string.Join("&", queryParams)}";
            }

            return baseEndpoint;
        }

        public IActionResult OnGetClearFilters()
        {
            return RedirectToPage("./Index");
        }

        public int GetTotalTasks() => Tasks.Count;
        public int GetInProgressTasks() => Tasks.Count(t => t.Status?.Equals("InProgress", StringComparison.OrdinalIgnoreCase) ?? false);
        public int GetCompletedTasks() => Tasks.Count(t => t.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) ?? false);
        public int GetOverdueTasks() => Tasks.Count(t => t.Deadline.HasValue && t.Deadline.Value < DateTime.Now &&
            !t.Status?.Equals("Completed", StringComparison.OrdinalIgnoreCase) == true);
    }
}