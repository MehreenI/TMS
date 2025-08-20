using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text;
using System.Text.Json;
using TMS.Models;

namespace TMS.Pages.Task
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;
        public List<UserDtos> Users { get; set; } = new();


        [BindProperty]
        public CreateTaskRequest Task { get; set; } = new();

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
                    await LoadUsers();
                    return Page();
                }

                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiEndpoint = $"{_apiUrl}/api/TaskItems";
                var json = JsonSerializer.Serialize(Task);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

             

                var response = await _httpClient.PostAsync(apiEndpoint, content);

               

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Task created successfully!";
                    return RedirectToPage("/Task/Index");
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
