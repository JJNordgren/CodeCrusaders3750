using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using CS3750Assignment1.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

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

            // Only store the Date
            User.BirthDate = User.BirthDate.Date;

            // Save the user to the database with the hashed password and salt
            _context.Users.Add(User);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToPage("/LoginPage");
        }

        public void UnitTestSuccessfulRegistration(User NewUser)
        {
            if (NewUser == null)
            {
                throw new ArgumentNullException(nameof(NewUser));
            }

            // Validate the user data
            var validationContext = new ValidationContext(NewUser);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(NewUser, validationContext, validationResults, true);

            if (!isValid)
            {
                throw new InvalidOperationException("Invalid user data: " + string.Join(", ", validationResults.Select(vr => vr.ErrorMessage)));
            }

            // Check if passwords match
            if (NewUser.password != NewUser.confirmPassword)
            {
                throw new InvalidOperationException("Passwords do not match");
            }

            // Generate a unique salt for each user
            string salt;
            NewUser.password = PasswordHelper.HashPassword(NewUser.password, out salt);
            NewUser.Salt = salt; // Store the generated salt in the database

            // Clear confirmPassword since it is not stored
            NewUser.confirmPassword = null;

            // Only store the Date
            NewUser.BirthDate = NewUser.BirthDate.Date;

            // Save the user to the database with the hashed password and salt
            _context.Users.Add(NewUser);
            _context.SaveChanges();
        }


    }
}

