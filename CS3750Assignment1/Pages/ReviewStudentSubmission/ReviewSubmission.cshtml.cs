using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.ReviewStudentSubmission
{
    public class ReviewSubmissionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReviewSubmissionModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Assignment Assignment { get; set; }
        public Submission Submission { get; set; }

        public IList<Submission> AllSubmissions { get; set; } = new List<Submission>();

        // An array to be used to make the pie chart      A   B  C  D  E
        public int[] NumStudentsPerGrade { get; set; } = [0, 0, 0, 0, 0];
        public IList<Submission> Submissions { get; set; } = new List<Submission>();
        public string LetterGrade { get; set; } = "";
        public int LetterGradeIndexNumber { get; set; } = -1;


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == id);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Get all submissions: needed for displaying pie chart
            AllSubmissions = await _context.Submissions.Where(s => s.AssignmentId == id).ToListAsync();
            List<Submission> gradedSubmissions = AllSubmissions.Where(s => s.Grade != null).ToList();

            // Load the latest submission for this assignment and current user.
            Submission = AllSubmissions
                .Where(s => s.UserId == userId.Value)
                .OrderByDescending(s => s.SubmissionTime)
                .FirstOrDefault();

            // If the student has a submission, make the pie chart
            List<double> listOfGrades = new List<double>();

            // Making a variable for student's percent grade for chart things
            double studentsPercentGrade = -1;

            foreach (var s in gradedSubmissions)
            {
                // First, let's turn the grade into a percentage:
                double percentGrade = ((double)s.Grade! / s.TotalPoints) * 100;
                listOfGrades.Add(percentGrade);

                // Now count how many grades fall under each category
                // [ A, B, C, D, E ]
                // [ 0, 1, 2, 3, 4 ]
                NumStudentsPerGrade[0] = listOfGrades.Count(x => x >= 90);
                NumStudentsPerGrade[1] = listOfGrades.Count(x => x >= 80 && x < 90);
                NumStudentsPerGrade[2] = listOfGrades.Count(x => x >= 70 && x < 80);
                NumStudentsPerGrade[3] = listOfGrades.Count(x => x >= 64 && x < 70);
                NumStudentsPerGrade[4] = listOfGrades.Count(x => x < 64);

                if (s.UserId == userId)
                {
                    studentsPercentGrade = percentGrade;
                }
            }

            
            if (studentsPercentGrade >= 90)
            {
                LetterGradeIndexNumber = 0;
                if (studentsPercentGrade >= 94)
                {
                    LetterGrade = "A";
                }
                else
                {
                    LetterGrade = "A-";
                }
            }
            else if (studentsPercentGrade >= 80)
            {
                LetterGradeIndexNumber = 1;
                if (studentsPercentGrade >= 84)
                {
                    if (studentsPercentGrade >= 87) { LetterGrade = "B+"; } else { LetterGrade = "B"; }
                }
                else
                {
                    LetterGrade = "B-";
                }
            }
            else if (studentsPercentGrade >= 70)
            {
                LetterGradeIndexNumber = 2;
                if (studentsPercentGrade >= 74)
                {
                    if (studentsPercentGrade >= 77) { LetterGrade = "C+"; } else { LetterGrade = "C"; }
                }
                else
                {
                    LetterGrade = "C-";
                }
            }
            else if (studentsPercentGrade >= 64)
            {
                LetterGradeIndexNumber = 3;
                if (studentsPercentGrade >= 67) { LetterGrade = "D+"; } else { LetterGrade = "D"; }
            }
            else if (studentsPercentGrade >= 0)
            {
                LetterGradeIndexNumber = 4;
                LetterGrade = "E";
            }
            else
            {
                LetterGrade = "N/A";
            }

            return Page();
        }

        // Removes the GUID prefix from stored file names.
        public string DisplayFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return fileName;
            }
            var parts = fileName.Split('_');
            if (parts.Length > 1)
            {
                // Rejoin everything except the first part (the GUID).
                return string.Join("_", parts.Skip(1));
            }
            return fileName;
        }
    }
}
