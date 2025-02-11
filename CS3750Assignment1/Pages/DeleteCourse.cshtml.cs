using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;

namespace CS3750Assignment1.Pages
{
    public class DeleteCourseModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteCourseModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get the logged-in instructor's ID
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            // Retrieve the course
            Course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (Course == null)
            {
                return NotFound();
            }

            // Ensure only the instructor can delete this course
            if (Course.InstructorId != userId.Value)
            {
                return Unauthorized();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Get the logged-in instructor's ID
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            var course = await _context.Courses
                .Include(c => c.Assignments) // Ensure Assignments are loaded
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            // Ensure only the instructor can delete this course
            if (course.InstructorId != userId.Value)
            {
                return Unauthorized();
            }

            // Cascade delete: Remove related records before deleting the course
            var registrations = _context.Registrations.Where(r => r.CourseId == id);
            _context.Registrations.RemoveRange(registrations);

            var submissions = _context.Submissions.Where(s => s.Assignment.CourseId == id);
            _context.Submissions.RemoveRange(submissions);

            var assignments = _context.Assignment.Where(a => a.CourseId == id);
            _context.Assignment.RemoveRange(assignments);

            // Remove the course
            _context.Courses.Remove(course);

            await _context.SaveChangesAsync();

            return RedirectToPage("./InstructorClasses");
        }
    }
}
