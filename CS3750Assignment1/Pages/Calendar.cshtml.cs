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

            if (!string.IsNullOrEmpty(userType))
            {
                if (userType == "T") // Instructor check
                {
                    Course = await _context.Courses
                        .Where(c => c.InstructorId == userId.Value)
                        .ToListAsync();
                }
                else if (userType == "S") // Student check
                {
                    Registrations = await _context.Registrations
                        .Include(r => r.Course)
                        .ToListAsync();
                }
            }

            return Page();
        }
        public async Task<JsonResult> OnGetGetEventsAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userType = HttpContext.Session.GetString("UserType");

            if (userId == null)
            {
                return new JsonResult(new { error = "Unauthorized" });
            }

            var events = new List<object>(); // ensure events are correctly added to event list
            DateTime semesterStart = new DateTime(DateTime.Today.Year, 1, 1); // January 1st of the current year
            DateTime semesterEnd = new DateTime(DateTime.Today.Year, 4, 30); // April 30th of the current year

            if (userType == "T") // Instructor View
            {
                var courses = await _context.Courses
                    .Where(c => c.InstructorId == userId.Value)
                    .ToListAsync();

                foreach (var course in courses)
                {
                    var meetingDays = course.MeetingDays?.ToArray() ?? Array.Empty<string>();

                    DateTime currentDate = semesterStart;
                    while (currentDate <= semesterEnd)
                    {
                        if (meetingDays.Contains(currentDate.DayOfWeek.ToString()))
                        {
                            events.Add(new
                            {
                                title = $"{course.Department} {course.ClassNumber}\n",
                                start = currentDate.Add(course.StartTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                                end = currentDate.Add(course.EndTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                                location = course.Location,
                                url = $"/ClassDash?id={course.Id}",
                                isAssignment = false // Not an assignment
                            });
                        }
                        currentDate = currentDate.AddDays(1);
                    }
                }
            }
            else if (userType == "S") // Student View
            {
                var registrations = await _context.Registrations
                    .Include(r => r.Course)
                    .ThenInclude(c => c.Assignments)
                    .Where(r => r.UserId == userId.Value)
                    .ToListAsync();

                foreach (var registration in registrations)
                {
                    var course = registration.Course;
                    var meetingDays = course.MeetingDays?.ToArray() ?? Array.Empty<string>();

                    DateTime currentDate = semesterStart;
                    while (currentDate <= semesterEnd)
                    {
                        if (meetingDays.Contains(currentDate.DayOfWeek.ToString()))
                        {
                            events.Add(new
                            {
                                title = $"{course.Department} {course.ClassNumber}\n",
                                start = currentDate.Add(course.StartTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                                end = currentDate.Add(course.EndTime).ToString("yyyy-MM-ddTHH:mm:ss"),
                                location = course.Location,
                                url = $"/ClassDash?id={course.Id}",
                                isAssignment = false // Not an assignment
                            });
                        }
                        currentDate = currentDate.AddDays(1);
                    }

                    // Process assignments within the same loop
                    if (course?.Assignments != null && course.Assignments.Any())
                    {
                        foreach (var assignment in course.Assignments)
                        {
                            // Ensure the DueDate is within the semester period
                            if (assignment.DueDate >= semesterStart && assignment.DueDate <= semesterEnd)
                            {
                                // Format the DueDate to include only the date part
                                var formattedDueDate = assignment.DueDate.ToString("yyyy-MM-dd");
                                var day = new System.TimeSpan(1, 0, 0, 0);
                                var formattedDueDatePlusADay = assignment.DueDate.Add(day).ToString("yyyy-MM-dd");
                                events.Add(new
                                {
                                    title = $"{course.Department} {course.ClassNumber}\n{assignment.Title}",
                                    start = formattedDueDate, // Use only the date part FIXME can't get this to work, makes everything disappear off calendar
                                    end = formattedDueDatePlusADay,
                                    allDay = true, // Set allDay to true
                                    url = $"/AssignmentDetails?id={assignment.Id}",
                                    isAssignment = true // This is an assignment
                                });
                            }
                        }
                    }
                }
            }

            return new JsonResult(events);
        }


    }
}
