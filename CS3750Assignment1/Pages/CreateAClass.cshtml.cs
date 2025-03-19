using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Stripe;

namespace CS3750Assignment1.Pages
{
    public class CreateAClassModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;
        private readonly ILogger<CreateAClassModel> _logger;

        public CreateAClassModel(CS3750Assignment1.Data.ApplicationDbContext context, ILogger<CreateAClassModel> logger)
        {
            _context = context;
            _logger = logger;
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

            // Create the class
            bool result = await CreateAClass(Course);

            // If create a class fails show the error
            if (!result)
            {
                ModelState.AddModelError("", "Failed to create the class. Please try again.");
                DepartmentOptions = new List<string> { "CS", "Math", "Science", "History", "Health" };
                return Page();
            }

            // Set the session flag when an instructor adds or deletes a course
            HttpContext.Session.SetString("InstructorCourseChanged", "true");

            return RedirectToPage("./InstructorClasses");
        }

        // Create a class and add it to the database
        public async Task<bool> CreateAClass(Course course)
        {
            try
            {
                // Add the course to the database   
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();

            }
            // Catch any errors that occur during the process
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a class.");
                return false;
            }

            return true;
            
        }
    }
}
