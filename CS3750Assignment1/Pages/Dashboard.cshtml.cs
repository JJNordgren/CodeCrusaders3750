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
    public class DashboardModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public DashboardModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            Course = await _context.Courses
                .Where(c => c.InstructorId == userId 
                /*|| c.Id == the_courseID_attached_to_registration_matching_userID*/).ToListAsync();

            

            return Page();
        }
    }
}
