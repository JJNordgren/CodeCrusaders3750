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

        [BindProperty]
        public Course Course { get; set; } = default!;

        public List<string> DepartmentOptions { get; set; }



        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Course.MeetingDays = Request.Form["MeetingDays"].ToList();
            Course.StartTime = TimeSpan.Parse(Request.Form["StartTime"]);
            Course.EndTime = TimeSpan.Parse(Request.Form["EndTime"]);

            _context.Courses.Add(Course);
            await _context.SaveChangesAsync();

            return RedirectToPage("./InstructorClasses");
        }
    }
}
