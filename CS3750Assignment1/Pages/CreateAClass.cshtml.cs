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
    public class CreateAClassModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public CreateAClassModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Course Course { get; set; } = default!;

        public List<string> DepartmentOptions { get; set; } = new List<string>();

        public IActionResult OnGet()
        {
            DepartmentOptions = new List<string>
            {
                "CS", "Math", "Science", "History", "Health"
            };

            Course = new Course();
            //ViewData["InstructorId"] = new SelectList(_context.Users, "Id", "password");
            return Page();
        }

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // Log or inspect errors
                }

                DepartmentOptions = new List<string> { "CS", "Math", "Science", "History", "Health" };

                return Page();
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            Course.InstructorId = userId.Value;

            Course.MeetingDays = Request.Form["Course.MeetingDays"].ToList();

            Course.Instructor = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
            if (Course.Instructor == null)
            {
                ModelState.AddModelError("", "Invalid instructor. Please check your login credentials.");
                DepartmentOptions = new List<string> { "CS", "Math", "Science", "History", "Health" };
                return Page();
            }

            _context.Courses.Add(Course);
            
            await _context.SaveChangesAsync();

            return RedirectToPage("./InstructorClasses");
        }
    }
}
