using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using TMS.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace TMS.Pages.Home
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public LoginModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiUrl = configuration["ApiUrl"] ?? "http://localhost:5019";
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        [TempData]
        public string? ErrorMessage { get; set; }

        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
            // Clear any existing error messages
            ErrorMessage = null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ErrorMessage = "Please correct the errors below.";
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(Input.Email) || string.IsNullOrWhiteSpace(Input.Password))
                {
                    ErrorMessage = "Email and password are required.";
                    return Page();
                }

                // Create login request
                var loginRequest = new LoginRequest
                {
                    Email = Input.Email.Trim(),
                    Password = Input.Password
                };

                // Call API directly
                var apiEndpoint = $"{_apiUrl}/api/Auth/login";
                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (authResponse != null && !string.IsNullOrWhiteSpace(authResponse.Token))
                    {
                        // Store authentication data in session
                        HttpContext.Session.SetString("JWTToken", authResponse.Token);
                        HttpContext.Session.SetString("UserId", authResponse.UserId.ToString());
                        HttpContext.Session.SetString("FullName", authResponse.FullName ?? "");
                        HttpContext.Session.SetString("Email", Input.Email);
                        HttpContext.Session.SetString("UserRole", authResponse.Role ?? "User");

                        SuccessMessage = "Login successful!";
                        return RedirectToPage("/Home/Index");
                    }
                }

                ErrorMessage = "Invalid email or password. Please try again.";
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}