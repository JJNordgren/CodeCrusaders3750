using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;

        private readonly ApplicationDbContext _context;

        public PrivacyModel(ILogger<PrivacyModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

		public User User { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync()
		{
			// Retrieve the user ID from the session
			var userId = HttpContext.Session.GetInt32("UserId");

			if (userId == null)
			{
				// If no session, redirect to the login page
				return RedirectToPage("/LoginPage");
			}


			var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId);
			if (user == null)
			{
				return NotFound();
			}
			else
			{
				User = user;
			}

			return Page();
		}
	}

}
