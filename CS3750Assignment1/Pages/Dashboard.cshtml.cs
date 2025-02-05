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

        public async Task<IActionResult> OnGetAsync()
        {
            // If we return to the Dashboard, clear the courseId from the session
            HttpContext.Session.Remove("CourseId");

            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            // Checks for an instructor and loads courses list
            if (userType == "T")
            {
                Course = await _context.Courses
                .Where(c => c.InstructorId == userId)
                .ToListAsync();
            }

            // Checks for a student and loads courses list and registrations list
            if (userType == "S")
            {
                //retrieves courses
                Course = await _context.Courses
                    .Include(c => c.Instructor)
                    .ToListAsync();

                //retrieves the registrations table
                Registrations = await _context.Registrations
                    .Where(r => r.UserId == userId)
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
