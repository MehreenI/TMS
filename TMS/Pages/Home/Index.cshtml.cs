using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using TMS.Models;
using System.Text.Json;

namespace TMS.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiUrl;

        public IndexModel(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiUrl = configuration["ApiUrl"] ?? "http://localhost:5019";
        }

        public UserDtos? CurrentUser { get; set; }
        public DashboardStats? DashboardStats { get; set; }
        public List<TaskDtos>? MyTasks { get; set; }
        public List<TaskDtos>? DueSoonTasks { get; set; }
        public List<AnnouncementData>? Announcements { get; set; }
        public bool IsAdmin { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                // Check if user is authenticated
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                var userId = HttpContext.Session.GetString("UserId");
                var fullName = HttpContext.Session.GetString("FullName");
                var email = HttpContext.Session.GetString("Email");

                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(fullName))

                CurrentUser = new UserDtos
                {
                    Id = int.Parse(userId),
                    Email = email ?? "",
                };


                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    var statsResponse = await _httpClient.GetAsync($"{_apiUrl}/api/TaskItems/dashboard-stats");
                    if (statsResponse.IsSuccessStatusCode)
                    {
                        var statsContent = await statsResponse.Content.ReadAsStringAsync();
                        DashboardStats = JsonSerializer.Deserialize<DashboardStats>(statsContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                
                    var tasksResponse = await _httpClient.GetAsync($"{_apiUrl}/api/TaskItems/my-tasks");
                    if (tasksResponse.IsSuccessStatusCode)
                    {
                        var tasksContent = await tasksResponse.Content.ReadAsStringAsync();
                        MyTasks = JsonSerializer.Deserialize<List<TaskDtos>>(tasksContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                
                    var dueSoonResponse = await _httpClient.GetAsync($"{_apiUrl}/api/TaskItems/due-soon?hours=24");
                    if (dueSoonResponse.IsSuccessStatusCode)
                    {
                        var dueSoonContent = await dueSoonResponse.Content.ReadAsStringAsync();
                        DueSoonTasks = JsonSerializer.Deserialize<List<TaskDtos>>(dueSoonContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
               
                    var announcementsResponse = await _httpClient.GetAsync($"{_apiUrl}/api/Announcements");
                    if (announcementsResponse.IsSuccessStatusCode)
                    {
                        var announcementsContent = await announcementsResponse.Content.ReadAsStringAsync();
                        Announcements = JsonSerializer.Deserialize<List<AnnouncementData>>(announcementsContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }
                catch (Exception)
                {
                    return RedirectToPage("/Error");

                }

                return Page();
            }
            catch (Exception)
            {
                return RedirectToPage("/Error");
            }
        }
    }
}
