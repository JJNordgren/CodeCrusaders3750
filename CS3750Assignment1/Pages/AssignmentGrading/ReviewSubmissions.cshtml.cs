using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.AssignmentGrading
{
    public class ReviewSubmissionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ReviewSubmissionsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // List of submissions for the assignment
        public IList<Submission> Submissions { get; set; } = new List<Submission>();

        // Assignment details needed for display
        public string AssignmentTitle { get; set; } = string.Empty;
        public DateTime AssignmentDueDate { get; set; }
        public int[] NumStudentsPerGrade { get; set; } = [0, 0, 0, 0, 0];


        public async Task<IActionResult> OnGetAsync(int assignmentId)
        {
            // Retrieve the assignment details (e.g., title and due date)
            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(a => a.Id == assignmentId);

            if (assignment == null)
            {
                return NotFound();
            }

            AssignmentTitle = assignment.Title;
            AssignmentDueDate = assignment.DueDate;

            // Retrieve all submissions for this assignment, including student details
            Submissions = await _context.Submissions
                .Where(s => s.AssignmentId == assignmentId)
                .Include(s => s.Student)
                .ToListAsync();

            var gradedSubmissions = Submissions.Where(s => s.Grade != null).ToList();
            foreach (var s in gradedSubmissions)
            {
                // First, let's turn the grade into a percentage:
                double? percentGrade = ((double)s.Grade! / s.TotalPoints) * 100;

                // Then stick it in a slot of the 5-element array NumStudentsPerGrade
                // [ A, B, C, D, E ]
                // [ 0, 1, 2, 3, 4 ]
                if (percentGrade == null)                      { /* Do Nothing */ }
                else if (90 <  percentGrade)                        { NumStudentsPerGrade[0]++; }
                else if (80 <= percentGrade && percentGrade < 90)   { NumStudentsPerGrade[1]++; }
                else if (70 <= percentGrade && percentGrade < 80)   { NumStudentsPerGrade[2]++; }
                else if (64 <= percentGrade && percentGrade < 70)   { NumStudentsPerGrade[3]++; }
                else if (percentGrade < 64)                         { NumStudentsPerGrade[4]++; }
            }

            return Page();
        }
    }
}
