using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;

namespace CS3750Assignment1.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public DashboardModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
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
                // Load courses for instructors
                Course = await _context.Courses
                    .Where(c => c.InstructorId == userId)
                    .ToListAsync();
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
                Assignments = await _context.Assignments
                    .Where(a => courseIds.Contains(a.CourseId) && a.DueDate >= DateTime.Now) // <-- Fix: Get assignments from all registered courses
                    .OrderBy(a => a.DueDate)
                    .ToListAsync();
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

        //public async Task<IActionResult> OnPostAsync()
        //{


        //    HttpContext.Session.SetInt32("CourseId", user.Id);

        //    return RedirectToPage("/ClassDash");
        //}
    }
}
