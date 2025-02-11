

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CS3750Assignment1.Pages
{
    public class EditCourseModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditCourseModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        public List<string> DepartmentOptions { get; set; } = new List<string> { "CS", "Math", "Science", "History", "Health" };

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

            // Ensure the course belongs to the logged-in instructor
            if (Course.InstructorId != userId.Value)
            {
                return Unauthorized();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                DepartmentOptions = new List<string> { "CS", "Math", "Science", "History", "Health" };
                return Page();
            }

            var existingCourse = await _context.Courses.FindAsync(Course.Id);
            if (existingCourse == null)
            {
                return NotFound();
            }

            // Ensure only the instructor who owns the course can edit it
            var userId = HttpContext.Session.GetInt32("UserId");
            if (existingCourse.InstructorId != userId)
            {
                return Unauthorized();
            }

            // Update course properties
            existingCourse.Department = Course.Department;
            existingCourse.Name = Course.Name;
            existingCourse.ClassNumber = Course.ClassNumber;
            existingCourse.CreditHours = Course.CreditHours;
            existingCourse.Capacity = Course.Capacity;
            existingCourse.Location = Course.Location;
            existingCourse.MeetingDays = Request.Form["Course.MeetingDays"].ToList();
            existingCourse.StartTime = Course.StartTime;
            existingCourse.EndTime = Course.EndTime;

            // Save changes
            _context.Courses.Update(existingCourse);
            await _context.SaveChangesAsync();

            return RedirectToPage("./InstructorClasses");
        }
    }
}
