using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages
{
    public class AssignmentSubmissionModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AssignmentSubmissionModel(ApplicationDbContext context)
        {
            _context = context;
        }
        public Assignment Assignment { get; set; }

        // Binds the submission form inputs.
        [BindProperty]
        public Submission Submission { get; set; } = new Submission();

        // Loads assignment details based on the id in the URL.
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int assignmentId = 1;
            // Retrieve the user id from session.
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Redirect to login (or error page) if the session is missing.
                return RedirectToPage("/LoginPage");
            }

            // Load the assignment details.
            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == assignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            // Pre-populate the submission with the assignment id and user id.
            Submission.AssignmentId = Assignment.Id;
            Submission.UserId = userId.Value;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Retrieve the user id from session again.
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }
            Submission.UserId = userId.Value;

            // Re-load the assignment details for verification.
            var assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == Submission.AssignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            // Validate based on the assignment type.
            if (assignment.AssignmentType == 'T' && string.IsNullOrWhiteSpace(Submission.TextSubmission))
            {
                ModelState.AddModelError("Submission.TextSubmission", "A text submission is required.");
            }
            // For file-based submissions (e.g., AssignmentType == 'F'), add file-specific validation if needed.

            if (!ModelState.IsValid)
            {
                // Re-populate the assignment details so the page can re-display correctly.
                Assignment = assignment;
                return Page();
            }

            // Set additional submission properties.
            Submission.SubmissionTime = DateTime.Now;
            Submission.TotalPoints = assignment.MaxPoints;
            // Grade remains null until reviewed.

            // Save the new submission.
            _context.Submissions.Add(Submission);
            await _context.SaveChangesAsync();

            // Redirect to a success/confirmation page.
            return RedirectToPage("/Home");
        }
    }
}
