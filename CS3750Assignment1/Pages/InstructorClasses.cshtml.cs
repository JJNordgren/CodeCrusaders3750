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
    public class InstructorClassesModel : PageModel
    {

        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public InstructorClassesModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }


        public IList<Course> Course { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            // Retrieve the user ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            Course = await _context.Courses
                .Where(c => c.InstructorId == userId) // Filter by InstructorId
                .ToListAsync();

            return Page();
        }
    }
}
