using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages
{
    public class ClassDashModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public ClassDashModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Assignment> Assignment { get;set; } = default!;

        /// <summary>
        /// We're going to need this in at least two places
        /// </summary>
        public string CourseName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            var userType = HttpContext.Session.GetString("UserType");
            var courseId = HttpContext.Session.GetInt32("CourseId");
            // id is the course ID, passed in when clicking the link from Dashboard
            // here we're verifying that we either clicked on the course, and thus have an id
            // or that there is a course id in session already
            if (id == null && courseId == null)
            {
                return NotFound();
            }

            // If we do have an id passed in, that means we just entered the page and need to start a session 
            if (id != null)
            {
                // setting the courseId for the session requires a non-nullable int, but id can be null
                // so to appease it, we'll create an actual int.
                // ?? just means that if id is null, use 0. Never will be thanks to the if statement above
                courseId = id;

                HttpContext.Session.SetInt32("CourseId", courseId.Value);
            }

            // Get the course name comprised of department and class number
            var course = await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new { c.Department, c.ClassNumber })
                .FirstOrDefaultAsync();

            Assignment = await _context.Assignment
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Course)
                .Include(a => a.Instructor)
                .ToListAsync();

            CourseName = $"{course.Department} {course.ClassNumber}";

            return Page();
        }
    }
}
