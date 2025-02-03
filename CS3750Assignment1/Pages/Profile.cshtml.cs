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

namespace CS3750Assignment1.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly CS3750Assignment1.Data.ApplicationDbContext _context;

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
                myuser.BirthDate = User.BirthDate;
                myuser.addressLine1 = User.addressLine1;
                myuser.addressLine2 = User.addressLine2;
                myuser.city = User.city;
                myuser.state = User.state;
                myuser.zip = User.zip;
                myuser.phone = User.phone;

            }

            

            _context.Users.Update(myuser);
            await _context.SaveChangesAsync();
           

            return Page();
        }
    }
             
}
