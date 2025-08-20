using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TMS.Pages.Home
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            try
            {
                HttpContext.Session.Clear();
                return RedirectToPage("/Home/Login");
            }
            catch
            {
                return RedirectToPage("/Home/Login");
            }
        }
    }
}
