using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.AssignmentGrading
{
    public class GradeAssignmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public GradeAssignmentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // The submissionId is passed in the URL.
        [BindProperty(SupportsGet = true)]
        public int SubmissionId { get; set; }

        // Holds the submission record (with its related Assignment).
        public Submission Submission { get; set; } = new Submission();

        // Holds the assignment details (can also be accessed via Submission.Assignment).
        public Assignment Assignment { get; set; } = new Assignment();

        // This is bound to the teacher’s input.
        [BindProperty]
        public int ReceivedPoints { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Load the submission along with its related assignment.
            Submission = await _context.Submissions
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == SubmissionId);

            if (Submission == null)
            {
                return NotFound();
            }

            Assignment = Submission.Assignment;

            // Optionally prefill the textbox with the current points (if any).
            ReceivedPoints = Submission.Grade ?? 0;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Retrieve the submission to update.
            var submissionToUpdate = await _context.Submissions
                .Include(s => s.Assignment)
                .FirstOrDefaultAsync(s => s.Id == SubmissionId);

            if (submissionToUpdate == null)
            {
                return NotFound();
            }

            // Validate that the received points do not exceed the assignment’s maximum.
            if (ReceivedPoints > submissionToUpdate.Assignment.MaxPoints)
            {
                ModelState.AddModelError("ReceivedPoints", "Received points cannot exceed the maximum points.");
                Assignment = submissionToUpdate.Assignment;
                Submission = submissionToUpdate;
                return Page();
            }

            // Update the submission with the teacher-entered points.
            submissionToUpdate.Grade = ReceivedPoints;
            // Optionally, you might also update the Grade field:
            // submissionToUpdate.Grade = ReceivedPoints;

            await _context.SaveChangesAsync();

            // Redirect back to the submissions list (or another page as needed).
            return RedirectToPage("/AssignmentGrading/ReviewSubmissions", new { assignmentId = submissionToUpdate.AssignmentId });
        }
    }
}
