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
    public class ClassDashModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public ClassDashModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Assignment> Assignment { get; set; } = new List<Assignment>();

        public IList<Submission> Submission { get; set; } = new List<Submission>();

        public IList<Submission> StudentSubmission { get; set; } = new List<Submission>();

        public double? CurrentGrade { get; set; } = null;
        public double TotalPoints { get; set; } = 0;
        public int[] OverallGrades { get; set; } = new int[5];

        public string LetterGrade { get; set; } = "";
        public int LetterGradeIndexNumber { get; set; } = -1;

        /// <summary>
        /// We're going to need this in at least two places
        /// </summary>
        public string CourseName { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // If no session, redirect to the login page
                return RedirectToPage("/LoginPage");
            }

            var userType = HttpContext.Session.GetString("UserType");
            var courseId = HttpContext.Session.GetInt32("CourseId");
            // id is the course ID, passed in when clicking the link from Dashboard
            // here we're verifying that we either clicked on the course, and thus have an id
            // or that there is a course id in session already
            if (id == null && courseId == null)
            {
                return NotFound();
            }

            // If we do have an id passed in, that means we just entered the page and need to start a session 
            if (id != null)
            {
                // setting the courseId for the session requires a non-nullable int, but id can be null
                // so to appease it, we'll create an actual int.
                // ?? just means that if id is null, use 0. Never will be thanks to the if statement above
                courseId = id;

                HttpContext.Session.SetInt32("CourseId", courseId.Value);
            }

            // Get the course name comprised of department and class number
            var course = await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new { c.Department, c.ClassNumber })
                .FirstOrDefaultAsync();

            // Get all assignments for this course
            Assignment = await _context.Assignment
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Course)
                .Include(a => a.Instructor)
                .ToListAsync();

            // Get all submissions of all students for assignments in the course
            var assignmentIDs = Assignment.Select(x => x.Id).ToList();
            Submission = await _context.Submissions
                .Where(x => assignmentIDs.Contains(x.AssignmentId))
                .ToListAsync();

            // Setting up variables for accumulating the total scores of all students who have submitted assignments
            var studentIds = Submission.Select(x => x.UserId).Distinct().ToList();
            List<double> listOfGrades = new List<double>();
            foreach (var studentId in studentIds)
            {
                // Get all submissions for the student
                var submission = Submission.Where(x => x.UserId == studentId).ToList();

                // Some might not yet be graded. For calculations of grades, let's remove the assignments that haven't been graded
                var GradedSubmissions = submission.Where(x => x.Grade != null).ToList();
                var sumOfPoints = GradedSubmissions.Sum(x => x.TotalPoints);
                var resultingGrade = 0.0;

                // There is a chance that between non-submissions and non-graded assignments, students might not have a grade in the class.
                if (GradedSubmissions.Count > 0)
                {
                    // Calculating grades:
                    // "messyGrade" because it isn't rounding yet
                    // ! is used to tell the compiler that we know it's not null, but it doesn't
                    double messyGrade = ((double)GradedSubmissions.Sum(x => x.Grade)! / sumOfPoints) * 100;
                    resultingGrade = Math.Round(messyGrade, 2);
                }
                else
                { // We check in cshtml that the grade is greater than 0, that's how we only display a total grade if there is at least one graded assignment
                    resultingGrade = -1;
                }

                // Only add the calculated grade to the list we'll count from for the overall grade chart if it was a valid grade
                if (resultingGrade != -1)
                {
                    listOfGrades.Add(resultingGrade);
                }

                // And grab the data for the user to be displayed on the class dash
                if (studentId == userId)
                {
                    StudentSubmission = submission;
                    TotalPoints = sumOfPoints;
                    CurrentGrade = resultingGrade;

                    // Find which letter the user's grade is
                    if (CurrentGrade >= 90)
                    {
                        LetterGrade = "A";
                        LetterGradeIndexNumber = 0;
                    }
                    else if (CurrentGrade >= 80)
                    {
                        LetterGrade = "B";
                        LetterGradeIndexNumber = 1;
                    }
                    else if (CurrentGrade >= 70)
                    {
                        LetterGrade = "C";
                        LetterGradeIndexNumber = 2;
                    }
                    else if (CurrentGrade >= 64)
                    {
                        LetterGrade = "D";
                        LetterGradeIndexNumber = 3;
                    }
                    else if (CurrentGrade >= 0)
                    {
                        LetterGrade = "E";
                        LetterGradeIndexNumber = 4;
                    }
                    else
                    {
                        LetterGrade = "N/A";
                    }
                }
            }

            OverallGrades[0] = listOfGrades.Count(x => x >= 90);
            OverallGrades[1] = listOfGrades.Count(x => x >= 80 && x < 90);
            OverallGrades[2] = listOfGrades.Count(x => x >= 70 && x < 80);
            OverallGrades[3] = listOfGrades.Count(x => x >= 64 && x < 70);
            OverallGrades[4] = listOfGrades.Count(x => x < 64);

            CourseName = $"{course.Department} {course.ClassNumber}";

            return Page();
        }
    }
}
