using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Linq;

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

        public IActionResult OnPost()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Please provide both email and password.");
                return Page();
            }

            var user = _context.Users.FirstOrDefault(u => u.email.ToLower() == Email.ToLower());

            if (user != null && user.password == Password)
            {
                // Pass user name to the Login page
                return RedirectToPage("/Login", new { userName = user.name });
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return Page();
        }

        public void OnGet()
        {

        }
    }
}
