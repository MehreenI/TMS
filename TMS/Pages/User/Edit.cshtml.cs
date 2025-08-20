using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TMS.Models.ViewModels;
using System.Text.Json;
using System.Text;
using SystemTask = System.Threading.Tasks.Task;

namespace TMS.Pages.User
{
    public class EditModel : PageModel
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration configuration;
        private readonly string ApiUrl;

        [BindProperty]
        public UserDtos UserModel { get; set; } = new();

        [TempData]
        public string Message { get; set; }

        public EditModel(HttpClient httpClient, IConfiguration configuration)
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

                foreach (var header in response.Headers)
                {
                    Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }

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
                            Message = "Failed to load user data.";
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
                Message = "Error loading user data.";
                return RedirectToPage("./Index");
            }
        }

        // POST: Update user data
        public async System.Threading.Tasks.Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid, returning Page()");
                return Page();
            }

            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (string.IsNullOrWhiteSpace(token))
                {
                    return RedirectToPage("/Home/Login");
                }

                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiEndpoint = $"{ApiUrl}/api/Users/{UserModel.Id}";
         

                var jsonContent = JsonSerializer.Serialize(UserModel);

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync(apiEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    Message = "User updated successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error updating user: {response.StatusCode} - {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
              
                ModelState.AddModelError(string.Empty, $"Error updating user: {ex.Message}");
                return Page();
            }
        }
    }
}