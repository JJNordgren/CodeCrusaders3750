using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CS3750Assignment1.Pages.AssignmentButtons
{
    public class EditAssignmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public EditAssignmentModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Assignment Assignment { get; set; } = default!;

        public IEnumerable<SelectListItem> AssignmentTypeList { get; set; } = new List<SelectListItem>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/LoginPage");

            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == id);
            if (Assignment == null)
                return NotFound();

            // Only the instructor who created the assignment can edit it
            if (Assignment.InstructorId != userId.Value)
                return Unauthorized();

            // Populate the select list for assignment types
            AssignmentTypeList = new List<SelectListItem>
        {
            new SelectListItem { Value = "T", Text = "Text Box" },
            new SelectListItem { Value = "F", Text = "File Upload" }
        };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/LoginPage");

            if (!ModelState.IsValid)
                return Page();

            var assignmentToUpdate = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == Assignment.Id);
            if (assignmentToUpdate == null)
                return NotFound();

            if (assignmentToUpdate.InstructorId != userId.Value)
                return Unauthorized();

            // Update the properties
            assignmentToUpdate.Title = Assignment.Title;
            assignmentToUpdate.Description = Assignment.Description;
            assignmentToUpdate.DueDate = Assignment.DueDate;
            assignmentToUpdate.MaxPoints = Assignment.MaxPoints;
            assignmentToUpdate.AssignmentType = Assignment.AssignmentType;

            _context.Assignment.Update(assignmentToUpdate);
            await _context.SaveChangesAsync();

            return RedirectToPage("/ClassDash");
        }
    }
}

