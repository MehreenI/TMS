using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text;
using System.Text.Json;

namespace TMS.Pages.User
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        [BindProperty]
        public CreateUserRequest UserModel { get; set; } = new();

        public CreateModel(HttpClient httpClient, IConfiguration configuration)
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
                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiEndpoint = $"{_apiUrl}/api/Users";
                var json = JsonSerializer.Serialize(UserModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

             

                var response = await _httpClient.PostAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Task created successfully!";
                    return RedirectToPage("/User/Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to create user.";
                    return Page();
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while creating the user.";
                return Page();
            }
        }
    }
}
