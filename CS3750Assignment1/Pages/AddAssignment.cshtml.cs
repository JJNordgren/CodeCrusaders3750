using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages
{
    public class AddAssignmentModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public AddAssignmentModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var courseId = HttpContext.Session.GetInt32("CourseId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }
            if (courseId == null)
            {
                // If no course for some reason, redirect to the dashboard
                return RedirectToPage("/Dashboard");
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ClassNumber");
            ViewData["InstructorId"] = new SelectList(_context.Users, "Id", "password");
            return Page();
        }

        [BindProperty]
        public Assignment Assignment { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            var cId = HttpContext.Session.GetInt32("CourseId");
            var uId = HttpContext.Session.GetInt32("UserId");

            if (cId == null || uId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            Assignment.CourseId = cId.Value;
            Assignment.InstructorId = uId.Value;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add the assignment to the database
            _context.Assignment.Add(Assignment);
            await _context.SaveChangesAsync();

            var course = await _context.Courses.FindAsync(cId.Value);

            var studentIds = _context.Registrations
                .Where(r => r.CourseId == cId.Value)
                .Select(r => r.UserId)
                .Distinct()
                .ToList();
            // Create notifications for all students in the course
            var studentsInCourse = _context.Users
                .Where(u => studentIds.Contains(u.Id))
                .ToList();

            foreach (var student in studentsInCourse)
            {
                var notification = new Notification
                {
                    CourseName = Assignment.Course.ClassNumber,
                    AssignmentName = Assignment.Title,
                    Message = "Created",
                    UserId = student.Id,
                    CourseId = Assignment.CourseId,
                    Deleted = false
                };

                _context.Notification.Add(notification);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToPage("./ClassDash");
        }
    }
}
