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
        ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "ClassNumber");
        ViewData["InstructorId"] = new SelectList(_context.Users, "Id", "password");
            return Page();
        }

        [BindProperty]
        public Assignment Assignment { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //these can be implemented later
            //Assignment.CourseId = int.Parse(HttpContext.Session.GetString("CourseId"));
            //Assignment.InstructorId = int.Parse(HttpContext.Session.GetString("UserId"));
            
            //hardcoded values for testing purposes
            Assignment.CourseId = 6;
            Assignment.InstructorId = 17;
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Assignment.Add(Assignment);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Home");
        }
    }
}
