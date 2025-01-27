using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Linq;
using CS3750Assignment1.Utilities;

namespace CS3750Assignment1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Clear session on visiting login page to ensure no leftover user session
            HttpContext.Session.Clear();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Please provide both email and password.");
                return Page();
            }

            var user = await _context.Users
                .Where(u => u.email == Email) //take out password for encryption
                .FirstOrDefaultAsync();

            if (user == null)
            {
                // Pass user name to the Login page
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return Page();
            }

            // In the absence of a salt stored in the database, use a static salt (or a different approach)
            string salt = "static_salt";  // Just an example; replace with proper dynamic salt generation
            bool isValid = PasswordHelper.VerifyPassword(Password, user.password, salt);

            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserType", user.userType.ToString());

            return RedirectToPage("/Home");
        }
    }
}
