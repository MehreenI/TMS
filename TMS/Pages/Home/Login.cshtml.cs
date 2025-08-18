using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TMS.Services;

namespace TMS.Pages.Home
{
    public class LoginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IAuthService authService, ILogger<LoginModel> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (await _authService.IsAuthenticatedAsync())
            {
                return RedirectToPage("/Home/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var isAuthenticated = await _authService.LoginAsync(Input.Email, Input.Password);

                if (isAuthenticated)
                {
                    _logger.LogInformation("User {Email} logged in successfully", Input.Email);
                    return RedirectToPage("/Home/Index");
                }
                else
                {
                    ErrorMessage = "Invalid email or password.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", Input.Email);
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }
    }

    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;
    }
}
