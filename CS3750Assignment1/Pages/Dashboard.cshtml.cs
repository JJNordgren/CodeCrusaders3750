using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CS3750Assignment1.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        // Add this to reimplement the logger, ILogger<DashboardModel> logger
        public DashboardModel(CS3750Assignment1.Data.ApplicationDbContext context) 
        {
            _context = context;

            // Initialize lists if they are null
            Course = Course ?? new List<Course>();
            Assignments = Assignments ?? new List<Assignment>();
            Registrations = Registrations ?? new List<Registration>();

            // Initialize JsonSerializerOptions with ReferenceHandler.Preserve
            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
        }

        public IList<Course> Course { get;set; } = default!;
        public IList<Registration> Registrations { get; set; } = default!;
        public IList<Assignment> Assignments { get; set; } = default!;


        public async Task<IActionResult> OnGetAsync()
        {
            // Clear stored courseId when returning to Dashboard
            HttpContext.Session.Remove("CourseId");

            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            if (userType == "T") 
            {
                // Check if the instructor's courses have changed
                var instructorCourseChanged = HttpContext.Session.GetString("InstructorCourseChanged"); //This only loads when Instructor logs in, when a instructor creates or deletes a course by checking flag
                if (instructorCourseChanged == "true")
                {
                    // Load courses for instructors
                    Course = await _context.Courses
                    .Where(c => c.InstructorId == userId)
                    .ToListAsync();

                    // Store the data in the session
                    HttpContext.Session.SetString("InstructorCourses", JsonSerializer.Serialize(Course, _jsonOptions));

                    // Set the flag to false after loading the data
                    HttpContext.Session.SetString("InstructorCourseChanged", "false");

                }
                else
                {
                    // Load the data from the previous session
                    var storedCourses = HttpContext.Session.GetString("InstructorCourses");
                    if (!string.IsNullOrEmpty(storedCourses))
                    {
                        Course = JsonSerializer.Deserialize<List<Course>>(storedCourses, _jsonOptions);
                    }
                }
            }
            else if (userType == "S")
            {
                // Load student's registered courses
                Registrations = await _context.Registrations
                    .Where(r => r.UserId == userId)
                    .ToListAsync();

                // Get the CourseIds the student is registered in
                var courseIds = Registrations.Select(r => r.CourseId).ToList();

                // Retrieve only the courses the student is registered in
                Course = await _context.Courses
                    .Where(c => courseIds.Contains(c.Id))
                    .Include(c => c.Instructor)
                    .ToListAsync();

                // Retrieve assignments for those courses
                Assignments = await _context.Assignment
                    .Where(a => courseIds.Contains(a.CourseId) && a.DueDate >= DateTime.Now)
                    .OrderBy(a => a.DueDate)
                    .ToListAsync();

                // Store the data in the session
                HttpContext.Session.SetString("StudentCourses", JsonSerializer.Serialize(Course, _jsonOptions));
                HttpContext.Session.SetString("StudentAssignments", JsonSerializer.Serialize(Assignments, _jsonOptions));

                // Set the flag to false after loading the data
                HttpContext.Session.SetString("StudentCourseChanged", "false");
            }
            else
            {
                // Load the data from the previous session
                var storedCourses = HttpContext.Session.GetString("StudentCourses");
                var storedAssignments = HttpContext.Session.GetString("StudentAssignments");

                if (!string.IsNullOrEmpty(storedCourses))
                {
                    Course = JsonSerializer.Deserialize<List<Course>>(storedCourses, _jsonOptions);
                }

                if (!string.IsNullOrEmpty(storedAssignments))
                {
                    Assignments = JsonSerializer.Deserialize<List<Assignment>>(storedAssignments, _jsonOptions);
                }
            }


            return Page();
        }

        //checks to see if the student is registered for a course
        public bool IsRegistered(int courseId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userId == null)
            {
                return false;
            }
            if (userType == "S")
            {
                return Registrations.Any(r => r.CourseId == courseId && r.UserId == userId.Value);
            }
            else
            {
                return true;
            }
        }
    }
}
