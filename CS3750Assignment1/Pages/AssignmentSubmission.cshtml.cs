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
using System.Collections.Generic;

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

        // For file upload (multiple files now)
        [BindProperty, ValidateNever]
        public List<IFormFile> UploadedFiles { get; set; } = new List<IFormFile>();

        // For accessing it as a model in the html page
        public int? CourseID { get; set; }

        // This flag tells the view whether to display the submission details or the form.
        public bool SubmissionCompleted { get; set; } = false;

        // OnGetAsync checks if a submission exists for the current user and assignment.
        public async Task<IActionResult> OnGetAsync(int? id, bool resubmit = false)
        {
            // First, ensure id is provided.
            if (id == null)
            {
                return NotFound();
            }
            int assignmentId = id.Value;

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }

            // Now load the assignment using the assignmentId from the URL.
            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == assignmentId);
            if (Assignment == null)
            {
                return NotFound();
            }

            var courseId = HttpContext.Session.GetInt32("CourseId");
            if (courseId != null)
            {
                CourseID = courseId;
            }
            else
            {
                CourseID = Assignment.CourseId;
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
            if (assignment.AssignmentType == 'F' && (UploadedFiles == null || !UploadedFiles.Any() || UploadedFiles.Sum(f => f.Length) == 0))
            {
                ModelState.AddModelError("UploadedFiles", "Please select at least one file.");
            }

            // Remove errors not applicable to the current type.
            if (assignment.AssignmentType == 'T')
            {
                ModelState.Remove("UploadedFiles");
            }
            else if (assignment.AssignmentType == 'F')
            {
                ModelState.Remove("Submission.TextSubmission");
            }

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
            if (assignment.AssignmentType == 'F' && UploadedFiles != null && UploadedFiles.Any())
            {
                if (Submission.FilePath == null)
                {
                    Submission.FilePath = new List<string>();
                }
                foreach (var file in UploadedFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "FileSubmissions");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        Submission.FilePath.Add(uniqueFileName);
                    }
                }
            }

            Submission.SubmissionTime = DateTime.Now;
            Submission.TotalPoints = assignment.MaxPoints;

            if (existingSubmission != null)
            {
                // Update the existing submission.
                existingSubmission.TextSubmission = Submission.TextSubmission;
                if (Submission.FilePath != null && Submission.FilePath.Any())
                {
                    // Replace the file list with the new one.
                    existingSubmission.FilePath = Submission.FilePath;
                }
                existingSubmission.SubmissionTime = Submission.SubmissionTime;
                Submission = existingSubmission;
            }
            else
            {
                _context.Submissions.Add(Submission);
            }

            await _context.SaveChangesAsync();

            // Redirect and pass the assignment id so that OnGetAsync receives it.
            return RedirectToPage(new { id = Submission.AssignmentId });
        }
    }
}
