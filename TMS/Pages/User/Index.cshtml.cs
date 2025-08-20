using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text.Json;
using System.Web;

namespace TMS.Pages.User
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public List<UserDtos> Users { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Role { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RegistrationPeriod { get; set; }

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiUrl = configuration["ApiUrl"] ?? "http://localhost:5019";
        }

        public async Task<IActionResult> OnGetAsync()
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

                var apiEndpoint = BuildApiEndpoint();

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

                        ApplyClientSideFilters();
                    }
                }

                return Page();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to load users.";
                return Page();
            }
        }

        private string BuildApiEndpoint()
        {
            var baseEndpoint = $"{_apiUrl}/api/Users";
            var queryParams = new List<string>();

            if (!string.IsNullOrWhiteSpace(Role))
            {
                queryParams.Add($"role={HttpUtility.UrlEncode(Role)}");
            }

            if (queryParams.Any())
            {
                return $"{baseEndpoint}?{string.Join("&", queryParams)}";
            }

            return baseEndpoint;
        }

        private void ApplyClientSideFilters()
        {
            // Search filter
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Users = Users.Where(u =>
                    (u.FirstName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.LastName?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (u.Email?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(RegistrationPeriod))
            {
                DateTime filterDate = RegistrationPeriod.ToLower() switch
                {
                    "today" => DateTime.Today,
                    "week" => DateTime.Today.AddDays(-7),
                    "month" => DateTime.Today.AddMonths(-1),
                    _ => DateTime.MinValue
                };

                if (filterDate != DateTime.MinValue)
                {
                    Users = Users.Where(u => u.CreatedAt >= filterDate).ToList();
                }
            }
        }

        public IActionResult OnGetClearFilters()
        {
            return RedirectToPage("./Index");
        }

        // Statistics methods
        public int GetTotalUsers() => Users.Count;
        public int GetActiveUsers() => Users.Count(u => u.LastLogin.HasValue);
        public int GetNewUsersThisMonth() => Users.Count(u => u.CreatedAt >= DateTime.Now.AddMonths(-1));
        public int GetAdminUsers() => Users.Count(u => u.Role?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false);
    }
}