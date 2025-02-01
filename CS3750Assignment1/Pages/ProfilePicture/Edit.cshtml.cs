using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;

namespace CS3750Assignment1.Pages.ProfilePicture
{
    public class EditModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        [BindProperty]
        public IFormFile? Image { get; set; }

        [BindProperty]
        public string? ImagePath { get; set; }

        public List<SelectListItem> ImageFiles { get; set; } = new List<SelectListItem>();

        public EditModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        //public User User { get; set; } = default!;
        public new User User { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                User = user;
            }

            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileImages");
            var imageFiles = Directory.GetFiles(imageDirectory)
                                      .Select(Path.GetFileName)
                                      .ToList();

            // Remove GUID from file names for display purposes
            ImageFiles = imageFiles.Select(file => new SelectListItem
            {
                Value = "/ProfileImages/" + file,
                Text = file.Substring(file.IndexOf('_') + 1) // Remove GUID part
            }).ToList();


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(User).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(User.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnPostUploadImageAsync()
        {
            if (Image != null)
            {
                var newFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Image.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ProfileImages", newFileName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                ImagePath = "/ProfileImages/" + newFileName;

                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null)
                {
                    return NotFound();
                }

                var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId);
                if (user == null)
                {
                    return NotFound();
                }
                else
                {
                    User = user;
                }

                // Update the user's profile picture path in the model
                User.picturePath = ImagePath;

                _context.Attach(User).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }


            return RedirectToPage(new { id = User.Id});
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

