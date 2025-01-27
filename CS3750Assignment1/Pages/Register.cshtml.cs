using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS3750Assignment1.Pages
{
    public class RegisterModel : PageModel
    {


        private readonly ApplicationDbContext _context;
        
        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        //create one bind property public user User {get; set;}

        [BindProperty]
        public User User { get; set; } = default!;
        
        /*
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public DateTime BirthDate { get; set; }*/

        //[BindProperty]
        //public char UserType { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // If not valid, return the form with validation messages
                return Page();
            }
            //change it to name = user.Name etc.
            var user = new User
            {
                firstName = User.firstName,
                lastName = User.lastName,
                email = User.email,
                password = User.password , 
                BirthDate = User.BirthDate,
                userType = User.userType
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToPage("/LoginPage");

        }
    }
}
