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
        public UserDtos SelectedUser { get; set; } = new();

        [TempData]
        public string Message { get; set; } = string.Empty;

        public EditModel(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
            ApiUrl = configuration["ApiUrl"] ?? string.Empty;

            // Debug: Print configuration values
            Console.WriteLine($"=== CONSTRUCTOR DEBUG ===");
            Console.WriteLine($"ApiUrl from configuration: '{ApiUrl}'");
            Console.WriteLine($"ApiUrl is null: {ApiUrl == null}");
            Console.WriteLine($"ApiUrl is empty: {string.IsNullOrEmpty(ApiUrl)}");
        }

        // GET: Load user data for editing
        public async System.Threading.Tasks.Task<IActionResult> OnGetAsync(int id)
        {
            Console.WriteLine($"=== OnGetAsync DEBUG ===");
            Console.WriteLine($"User ID parameter: {id}");
            Console.WriteLine($"ApiUrl value: '{ApiUrl}'");

            try
            {
                var apiEndpoint = $"{ApiUrl}api/Users/{id}";

                // Debug: Print the complete endpoint
                Console.WriteLine($"Complete API endpoint: '{apiEndpoint}'");
                Console.WriteLine($"Endpoint length: {apiEndpoint?.Length}");

                // Debug: Print HttpClient info
                Console.WriteLine($"HttpClient BaseAddress: {httpClient.BaseAddress}");
                Console.WriteLine($"HttpClient Timeout: {httpClient.Timeout}");

                Console.WriteLine("Sending GET request...");
                var response = await httpClient.GetAsync(apiEndpoint);

                // Debug: Print response info
                Console.WriteLine($"Response received!");
                Console.WriteLine($"Status Code: {response.StatusCode}");
                Console.WriteLine($"Status Code (numeric): {(int)response.StatusCode}");
                Console.WriteLine($"Is Success: {response.IsSuccessStatusCode}");
                Console.WriteLine($"Reason Phrase: '{response.ReasonPhrase}'");

                // Debug: Print response headers
                Console.WriteLine("Response Headers:");
                foreach (var header in response.Headers)
                {
                    Console.WriteLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }

                // Debug: Print response content
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content Length: {responseContent?.Length ?? 0}");
                Console.WriteLine($"Response Content: '{responseContent}'");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Response is successful, attempting deserialization...");

                    try
                    {
                        var user = await response.Content.ReadFromJsonAsync<UserDtos>();

                        if (user != null)
                        {
                            SelectedUser = user;
                            Console.WriteLine($"User deserialized successfully:");
                            Console.WriteLine($"  ID: {SelectedUser.Id}");
                            Console.WriteLine($"  Name: '{SelectedUser.FullName}'");
                            Console.WriteLine($"  Email: '{SelectedUser.Email}'");
                            Console.WriteLine($"  Role: '{SelectedUser.Role}'");
                            Console.WriteLine("Returning Page()...");
                            return Page();
                        }
                        else
                        {
                            Console.WriteLine("ERROR: User deserialization returned null!");
                            Message = "Failed to load user data.";
                            return RedirectToPage("./Index");
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
                        Console.WriteLine($"JSON Content: {responseContent}");
                        Message = "Error parsing user data.";
                        return RedirectToPage("./Index");
                    }
                }
                else
                {
                    Console.WriteLine($"ERROR: API returned non-success status: {response.StatusCode}");
                    Console.WriteLine($"Response content: {responseContent}");

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Message = "User not found.";
                    }
                    else
                    {
                        Message = $"Error loading user: {response.StatusCode}";
                    }

                    Console.WriteLine($"Redirecting to Index with message: '{Message}'");
                    return RedirectToPage("./Index");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Exception: {httpEx.Message}");
                Console.WriteLine($"Inner Exception: {httpEx.InnerException?.Message}");
                ModelState.AddModelError(string.Empty, $"Network error: {httpEx.Message}");
                Message = "Network error loading user data.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"Error retrieving user: {ex.Message}");
                Message = "Error loading user data.";
                return RedirectToPage("./Index");
            }
        }

        // POST: Update user data
        public async System.Threading.Tasks.Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine($"=== OnPostAsync DEBUG ===");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid, returning Page()");
                return Page();
            }

            try
            {
                var apiEndpoint = $"{ApiUrl}api/Users/{SelectedUser.Id}";
                Console.WriteLine($"PUT API endpoint: '{apiEndpoint}'");
                Console.WriteLine($"User ID: {SelectedUser.Id}");
                Console.WriteLine($"User Name: '{SelectedUser.FullName}'");
                Console.WriteLine($"User Email: '{SelectedUser.Email}'");

                // Serialize user data to JSON
                var jsonContent = JsonSerializer.Serialize(SelectedUser);
                Console.WriteLine($"JSON Content: {jsonContent}");

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                Console.WriteLine("Sending PUT request...");
                var response = await httpClient.PutAsync(apiEndpoint, content);

                Console.WriteLine($"PUT Response Status: {response.StatusCode}");
                Console.WriteLine($"PUT Is Success: {response.IsSuccessStatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Update successful!");
                    Message = "User updated successfully!";
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"PUT Error Content: {errorContent}");
                    ModelState.AddModelError(string.Empty, $"Error updating user: {response.StatusCode} - {errorContent}");
                    return Page();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"POST Exception: {ex.Message}");
                Console.WriteLine($"POST Stack Trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, $"Error updating user: {ex.Message}");
                return Page();
            }
        }
    }
}