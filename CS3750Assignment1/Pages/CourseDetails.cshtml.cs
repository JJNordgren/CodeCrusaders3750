using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;

namespace CS3750Assignment1.Pages
{
    public class CourseDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CourseDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Course Course { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get the logged-in instructor's ID
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            // Retrieve the course by ID
            Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (Course == null)
            {
                return NotFound();
            }

            // Ensure the logged-in user is the instructor
            if (Course.InstructorId != userId.Value)
            {
                return Unauthorized();
            }

            return Page();
        }
    }
}
