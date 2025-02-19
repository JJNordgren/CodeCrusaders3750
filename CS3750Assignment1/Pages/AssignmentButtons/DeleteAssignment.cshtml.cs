using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;

namespace CS3750Assignment1.Pages.AssignmentButtons
{
    public class DeleteAssignmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public DeleteAssignmentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Assignment Assignment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/LoginPage");

            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == id);
            if (Assignment == null)
                return NotFound();

            if (Assignment.InstructorId != userId.Value)
                return Unauthorized();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/LoginPage");

            var assignment = await _context.Assignment
                .FirstOrDefaultAsync(a => a.Id == id);

            if (assignment == null)
                return NotFound();

            if (assignment.InstructorId != userId.Value)
                return Unauthorized();

            // Cascade delete: remove related submissions first
            var submissions = _context.Submissions.Where(s => s.AssignmentId == id);
            _context.Submissions.RemoveRange(submissions);

            _context.Assignment.Remove(assignment);
            await _context.SaveChangesAsync();

            return RedirectToPage("/ClassDash");
        }
    }
}
