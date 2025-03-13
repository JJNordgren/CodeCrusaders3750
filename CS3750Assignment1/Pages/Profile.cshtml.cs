using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using CS3750Assignment1.Utilities;
using Microsoft.IdentityModel.Tokens;

namespace CS3750Assignment1.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

        public string userPassword { get; set; }

        [BindProperty]
        public IFormFile? Image { get; set; }

        [BindProperty]
        public string? ImagePath { get; set; }

        public List<SelectListItem> ImageFiles { get; set; } = new List<SelectListItem>();

        public ProfileModel(CS3750Assignment1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        //public IActionResult OnGet()
        //{
        //    return Page();
        //}

        [BindProperty]
        public User User { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToPage("/LoginPage");
            }
            var user = await _context.Users.FirstOrDefaultAsync();
            if (user == null)
            {
                return RedirectToPage("/loginPage");
            }
            else
            {
                User = user;
            }


            User = await _context.Users.FindAsync(userId);
            //await _context.SaveChangesAsync();

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

            // Filler for password
            userPassword = "********";

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            //Selecting the user to update
            var myuser = await _context.Users.FirstOrDefaultAsync(x => x.email == User.email);
           

            //Updating the values based on what is on the form
            if (myuser != null)
            {
                myuser.firstName = User.firstName;
                myuser.lastName = User.lastName;
                // Only retrieve the Date
                myuser.BirthDate = User.BirthDate.Date;
                myuser.addressLine1 = User.addressLine1;
                myuser.addressLine2 = User.addressLine2;
                myuser.city = User.city;
                myuser.state = User.state;
                myuser.zip = User.zip;
                myuser.phone = User.phone;

                if (!string.IsNullOrEmpty(User.password) && User.password != "********")
                {
                    string salt;
                    myuser.password = PasswordHelper.HashPassword(User.password, out salt);
                    myuser.Salt = salt;
                }

            }
            //If no user exists redirect to main
            else
            {
                return RedirectToPage("./Index");
            }




            _context.Users.Update(myuser);
            await _context.SaveChangesAsync();

            // Filler for password
            userPassword = "********";


            //return Page();
            return RedirectToPage(new { id = User.Id });
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
                HttpContext.Session.SetString("PicturePath", ImagePath);

                _context.Attach(User).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }


            return RedirectToPage(new { id = User.Id });
        }

    }
             
}
