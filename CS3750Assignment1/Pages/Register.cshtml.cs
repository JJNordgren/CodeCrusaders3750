using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using CS3750Assignment1.Utilities;
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
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // If not valid, return the form with validation messages
                return Page();
            }

            // Generate a unique salt for each user
            string salt;
            User.password = PasswordHelper.HashPassword(User.password, out salt);
            User.Salt = salt; // Store the generated salt in the database

            // Clear confirmPassword since it is not stored
            User.confirmPassword = null;

            // Save the user to the database with the hashed password and salt
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToPage("/LoginPage");


            //change it to name = user.Name etc.
            /*
            var user = new User
            {
                firstName = User.firstName,
                lastName = User.lastName,
                email = User.email,
                password = User.password ,
                confirmPassword = User.confirmPassword,
                BirthDate = User.BirthDate,
                userType = User.userType
            };*/
        }
    }
}

