using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages
{
    public class CancelModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public IList<Course> Course { get; set; } = default!;
        public IList<CS3750Assignment1.Models.Registration> Registrations { get; set; } = default!;
        public int Credits { get; set; }
        public double Tuition { get; set; }

        [BindProperty]
        public long amount { get; set; } = default!;

        public double totalAmountPaid { get; set; }

        public CancelModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (user == null)
            {
                return RedirectToPage("/LoginPage");
            }

            user.amountPaid += amount;
            await _context.SaveChangesAsync();
            totalAmountPaid = user.amountPaid ?? 0.0;


            // Retrieves courses
            Course = await _context.Courses
                .Include(c => c.Instructor)
                .ToListAsync();

            // Retrieves the registrations table
            Registrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .ToListAsync();
            
            Credits = CalculateCredits(userId.Value);
            Tuition = Credits * 100;
            Tuition = Tuition - totalAmountPaid;

            return Page();
        }

        public int CalculateCredits(int userId)
        {
            var userType = HttpContext.Session.GetString("UserType");

            int credits = 0;

            if (userType == "S")
            {
                var studentRegistrations = Registrations.Where(r => r.UserId == userId);

                foreach (var registration in studentRegistrations)
                {
                    credits += registration.Course.CreditHours;
                }
            }

            return credits;
        }
    }
}
