using CS3750Assignment1.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages
{
    //was public class LogingModel : PageModel
    public class HomeModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HomeModel(ApplicationDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// A string to hold the username, will most likely go bye bye as the project progresses
        /// </summary>
        public string UserName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the UserId from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // Redirect to the login page if the session is invalid
                return RedirectToPage("/LoginPage");
            }

            // Query the database for the user's first and last name
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.firstName, u.lastName })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                // Redirect to the login page if the user is not found
                return RedirectToPage("/LoginPage");
            }

            // Set the UserName property to display on the page
            UserName = $"{user.firstName} {user.lastName}";

            return Page();
        }
    }
}
