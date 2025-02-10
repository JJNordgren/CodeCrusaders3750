using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace CS3750Assignment1.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public CalendarModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }
        public IList<Course> Course { get; set; } = new List<Course>();
        public IList<Registration> Registrations { get; set; } = new List<Registration>();


        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            if (!string.IsNullOrEmpty(userType) && userType == "T") // Instructor check
            {
                Course = await _context.Courses
                    .Where(c => c.InstructorId == userId.Value) // Use userId.Value safely
                    .ToListAsync();
            }

            return Page();
        }
        public async Task<JsonResult> OnGetGetEventsAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userId == null || userType != "T")
            {
                return new JsonResult(new { error = "Unauthorized" });
            }

            var courses = await _context.Courses
                .Where(c => c.InstructorId == userId.Value)
                .ToListAsync();

            // Set an end date for the semester (e.g., 3 months from now)
            DateTime semesterEnd = DateTime.Today.AddMonths(3);

            var events = new List<object>();

            foreach (var course in courses)
            {
                // Parse the meeting days (e.g., "Monday,Wednesday,Friday")
                // Parse the meeting days safely (e.g., "Monday,Wednesday,Friday")
                // Ensure MeetingDays is not null, then join into a comma-separated string
                var meetingDaysString = course.MeetingDays != null ? string.Join(",", course.MeetingDays) : "";
                var meetingDays = meetingDaysString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                // Start from today and loop until semester end
                DateTime currentDate = DateTime.Today;
                while (currentDate <= semesterEnd)
                {
                    // Check if today matches any meeting day
                    if (meetingDays.Contains(currentDate.DayOfWeek.ToString()))
                    {
                        events.Add(new
                        {
                            title = $"{course.Department} {course.ClassNumber}\n",
                            start = currentDate.Add(course.StartTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                            end = currentDate.Add(course.EndTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                            location = course.Location,
                            url = $"/ClassDash?id={course.Id}"
                        });
                    }

                    // Move to the next day
                    currentDate = currentDate.AddDays(1);
                }
            }

                return new JsonResult(events);
        }


        [BindProperty]
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
