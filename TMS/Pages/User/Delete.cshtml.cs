using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text.Json;

namespace TMS.Pages.User
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        [BindProperty]
        public UserDtos UserModel { get; set; } = new();

        [TempData]
        public string Message { get; set; }

        public DeleteModel(HttpClient httpClient, IConfiguration configuration)
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

                var apiEndpoint = $"{_apiUrl}/api/Users/{id}";
                var response = await _httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var user = JsonSerializer.Deserialize<UserDtos>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (user != null)
                    {
                        UserModel = user;
                        return Page();
                    }
                }

                Message = "User not found.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Message = "Error loading user data.";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(int id)
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

                var apiEndpoint = $"{_apiUrl}/api/Users/{id}";
                var response = await _httpClient.DeleteAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    Message = "User deleted successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error deleting user: {response.StatusCode} - {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error deleting user: {ex.Message}");
                return Page();
            }
        }
    }
}
