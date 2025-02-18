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

            return Page();
        }
    }
}
