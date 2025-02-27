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

            // Load the latest submission for this assignment and current user.
            Submission = await _context.Submissions
                .Where(s => s.AssignmentId == id && s.UserId == userId.Value)
                .OrderByDescending(s => s.SubmissionTime)
                .FirstOrDefaultAsync();

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
