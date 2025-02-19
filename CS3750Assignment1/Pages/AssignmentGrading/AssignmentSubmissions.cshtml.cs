using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.AssignmentGrading
{
    public class AssignmentSubmissionsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AssignmentSubmissionsModel(ApplicationDbContext context)
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


            foreach (var s in Submissions)
            {
                if      (s.Grade == null)                { /* Do Nothing */ }
                else if (90 < s.Grade)                   { NumStudentsPerGrade[0]++; } // A
                else if (80 <= s.Grade && s.Grade < 90)  { NumStudentsPerGrade[1]++; } // B
                else if (70 <= s.Grade && s.Grade < 80)  { NumStudentsPerGrade[2]++; } // C
                else if (64 <= s.Grade && s.Grade < 70)  { NumStudentsPerGrade[3]++; } // D
                else if (s.Grade < 64)                   { NumStudentsPerGrade[4]++; } // E
            }

            return Page();
        }
    }
}
