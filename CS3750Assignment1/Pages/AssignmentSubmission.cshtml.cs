using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CS3750Assignment1.Pages
{
    public class AssignmentSubmissionModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AssignmentSubmissionModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public Assignment Assignment { get; set; }

        // This property will hold the submission either as a new record or an existing one.
        [BindProperty]
        public Submission Submission { get; set; } = new Submission();

        // For file upload (currently one file)
        [BindProperty, ValidateNever]
        public IFormFile UploadedFile { get; set; }

        // This flag tells the view whether to display the submission details or the form.
        public bool SubmissionCompleted { get; set; } = false;

        // OnGetAsync checks if a submission exists for the current user and assignment.
        public async Task<IActionResult> OnGetAsync(int? id, bool resubmit = false)
        {
            // For testing, the assignment ID is hardcoded to 1.
            int assignmentId = 1;
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == assignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            if (!resubmit)
            {
                // Check for an existing submission.
                var existingSubmission = await _context.Submissions
                    .Where(s => s.AssignmentId == assignmentId && s.UserId == userId.Value)
                    .OrderByDescending(s => s.SubmissionTime)
                    .FirstOrDefaultAsync();

                if (existingSubmission != null)
                {
                    Submission = existingSubmission;
                    SubmissionCompleted = true;
                }
                else
                {
                    // No submission exists yet.
                    Submission = new Submission { AssignmentId = Assignment.Id, UserId = userId.Value };
                    SubmissionCompleted = false;
                }
            }
            else
            {
                // "Resubmit" was clicked – reset the form.
                Submission = new Submission { AssignmentId = Assignment.Id, UserId = userId.Value };
                SubmissionCompleted = false;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }
            Submission.UserId = userId.Value;

            var assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == Submission.AssignmentId);
            if (assignment == null)
            {
                return NotFound();
            }

            // Validate text submissions for text-type assignments.
            if (assignment.AssignmentType == 'T' && string.IsNullOrWhiteSpace(Submission.TextSubmission))
            {
                ModelState.AddModelError("Submission.TextSubmission", "A text submission is required.");
            }

            // Validate file submissions for file-type assignments.
            if (assignment.AssignmentType == 'F' && (UploadedFile == null || UploadedFile.Length == 0))
            {
                ModelState.AddModelError("UploadedFile", "Please select at least one file.");
            }

            // Now remove errors for fields that are not applicable.
            if (assignment.AssignmentType == 'T')
            {
                // For text assignments, we ignore file errors.
                ModelState.Remove("UploadedFile");
            }
            else if (assignment.AssignmentType == 'F')
            {
                // For file assignments, ignore text submission errors.
                ModelState.Remove("Submission.TextSubmission");
            }

            // If there are any errors, reload assignment info and return.
            if (!ModelState.IsValid)
            {
                Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == Submission.AssignmentId);
                return Page();
            }

            // Retrieve any existing submission for this user and assignment.
            var existingSubmission = await _context.Submissions
                .Where(s => s.AssignmentId == Submission.AssignmentId && s.UserId == userId.Value)
                .OrderByDescending(s => s.SubmissionTime)
                .FirstOrDefaultAsync();

            // Process file upload if assignment is file-type.
            if (assignment.AssignmentType == 'F' && UploadedFile != null && UploadedFile.Length > 0)
            {
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(UploadedFile.FileName);
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "FileSubmissions");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await UploadedFile.CopyToAsync(fileStream);
                }
                if (Submission.FilePath == null)
                {
                    Submission.FilePath = new System.Collections.Generic.List<string>();
                }
                Submission.FilePath.Add(uniqueFileName);
            }

            Submission.SubmissionTime = DateTime.Now;
            Submission.TotalPoints = assignment.MaxPoints;

            if (existingSubmission != null)
            {
                // Update the existing submission.
                existingSubmission.TextSubmission = Submission.TextSubmission;
                if (Submission.FilePath != null && Submission.FilePath.Any())
                {
                    existingSubmission.FilePath = Submission.FilePath;
                }
                existingSubmission.SubmissionTime = Submission.SubmissionTime;
                Submission = existingSubmission;
            }
            else
            {
                // Create a new submission record.
                _context.Submissions.Add(Submission);
            }

            await _context.SaveChangesAsync();

            // Use a redirect so the GET reloads the saved submission.
            return RedirectToPage();
        }

    }
}
