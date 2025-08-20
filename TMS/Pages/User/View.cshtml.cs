using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;

namespace TMS.Pages.User
{
    public class ViewModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string ApiUrl;

        [BindProperty]
        public UserDtos UserModel { get; set; } = new();

        [TempData]
        public string Message { get; set; }

        public ViewModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            ApiUrl = configuration["ApiUrl"];

        }
        public async System.Threading.Tasks.Task<IActionResult> OnGetAsync(int id)
        {

            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                
                var apiEndpoint = $"{ApiUrl}/api/Users/{id}";
                var response = await httpClient.GetAsync(apiEndpoint);

                // Debug: Print response content
                var responseContent = await response.Content.ReadAsStringAsync();
               

                if (response.IsSuccessStatusCode)
                {

                    try
                    {
                        UserModel = await response.Content.ReadFromJsonAsync<UserDtos>();

                        if (UserModel != null)
                        {
                            return Page();
                        }
                        else
                        {
                            return RedirectToPage("./Index");
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        Message = "Error parsing user data.";
                        return RedirectToPage("./Index");
                    }
                }
                else
                {

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Message = "User not found.";
                    }
                    else
                    {
                        Message = $"Error loading user: {response.StatusCode}";
                    }

                    return RedirectToPage("./Index");
                }
            }
            
            catch (Exception ex)
            {
               
                ModelState.AddModelError(string.Empty, $"Error retrieving user: {ex.Message}");
                Message = "Error loading user data.";
                return RedirectToPage("./Index");
            }
        }
    }
}
