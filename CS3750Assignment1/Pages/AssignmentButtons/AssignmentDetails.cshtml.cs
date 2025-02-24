using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Http;

namespace CS3750Assignment1.Pages.AssignmentButtons
{
    public class AssignmentDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public AssignmentDetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Assignment Assignment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/LoginPage");

            Assignment = await _context.Assignment.FirstOrDefaultAsync(a => a.Id == id);
            if (Assignment == null)
                return NotFound();


            return Page();
        }
    }
}
