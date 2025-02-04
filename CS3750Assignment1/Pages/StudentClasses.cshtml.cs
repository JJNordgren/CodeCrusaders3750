using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;

namespace CS3750Assignment1.Pages
{
    public class StudentClassesModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;
        private readonly ILogger<StudentClassesModel> _logger;

        public StudentClassesModel(CS3750Assignment1.Data.ApplicationDbContext context, ILogger<StudentClassesModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IList<Course> Course { get; set; } = default!;
        public IList<Registration> Registrations { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            await LoadCoursesAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync(int courseId)
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            _logger.LogInformation("User ID: {UserId}, Course ID: {CourseId}", userId, courseId);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
            if (course == null)
            {
                ModelState.AddModelError("", "Invalid course.");
                await LoadCoursesAsync(userId.Value);
                return Page();
            }

            var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.Value);
            if (student == null)
            {
                ModelState.AddModelError("", "Invalid student.");
                await LoadCoursesAsync(userId.Value);
                return Page();
            }

            var registration = new Registration
            {
                UserId = userId.Value,
                Student = student,
                CourseId = courseId,
                Course = course
            };

            _context.Registrations.Add(registration);
            _logger.LogInformation("Adding registration: {Registration}", registration);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Registration saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving registration.");
                ModelState.AddModelError("", "Error saving registration.");
                await LoadCoursesAsync(userId.Value);
                return Page();
            }

            // Re-load the courses after form submission
            await LoadCoursesAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostDropAsync(int courseId)
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            _logger.LogInformation("User ID: {UserId}, Course ID: {CourseId}", userId, courseId);

            var registration = await _context.Registrations
                .FirstOrDefaultAsync(r => r.CourseId == courseId && r.UserId == userId.Value);
            if (registration == null)
            {
                ModelState.AddModelError("", "Registration not found.");
                await LoadCoursesAsync(userId.Value);
                return Page();
            }

            _context.Registrations.Remove(registration);
            _logger.LogInformation("Removing registration: {Registration}", registration);

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Registration removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing registration.");
                ModelState.AddModelError("", "Error removing registration.");
                await LoadCoursesAsync(userId.Value);
                return Page();
            }

            // Re-load the courses after form submission
            await LoadCoursesAsync(userId.Value);
            return Page();
        }

        private async Task LoadCoursesAsync(int userId)
        {
            Course = await _context.Courses
                .Include(c => c.Instructor)
                .ToListAsync();

            Registrations = await _context.Registrations
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public bool IsRegistered(int courseId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return false;
            }

            return Registrations.Any(r => r.CourseId == courseId && r.UserId == userId.Value);
        }
    }
}