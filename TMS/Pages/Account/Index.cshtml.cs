using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;

namespace TMS.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        [BindProperty]
        public UpdateUserRequest UserProfile { get; set; } = new();

        [BindProperty]
        public ChangePasswordRequest PasswordChange { get; set; } = new();

        public UserDtos? CurrentUser { get; set; }

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

                var apiEndpoint = $"{_apiUrl}/api/Users/me";
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
                        CurrentUser = user;

                        UserProfile = new UpdateUserRequest
                        {
                            Id = user.Id,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            PhoneNumber = user.PhoneNumber,
                            Role = user.Role
                        };
                    }
                }
                return Page();
            }
            catch (Exception)
            {
                return Page();
            }
        }

        public async System.Threading.Tasks.Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var userIdString = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrWhiteSpace(userIdString) || !int.TryParse(userIdString, out var userId))
                {
                    ModelState.AddModelError(string.Empty, "Unable to determine current user.");
                    return Page();
                }

                var apiEndpoint = $"{_apiUrl}/api/Users/me";
                var jsonContent = JsonSerializer.Serialize(UserProfile);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PutAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToPage("/Account/Index");
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Failed to update profile: {response.StatusCode} - {error}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating profile: {ex.Message}");
                return Page();
            }
        }
    }
}
