using CS3750Assignment1.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CS3750Assignment1.Models
{
    public class User
    {
        ///auto generated Id field for the database items
        public int Id { get; set; }
        public string email {  get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; } = string.Empty;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string password { get; set; }

        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? confirmPassword { get; set; }

        [Required]
        [MinAge(16, ErrorMessage = "You must be at least 16 years old.")]
        public DateTime BirthDate { get; set; }

        public string? addressLine1 { get; set; } = string.Empty;
        public string? addressLine2 { get; set; } = string.Empty;
        public string? city { get; set; } = string.Empty;
        public string? state { get; set; } = string.Empty;
        public string? zip { get; set; } = string.Empty;
        public string? phone { get; set; } = string.Empty;

        /// Dictionary to store notification data
        //public Dictionary<string, string>? notification { get; set; }

        /// Stores a Char for user type, T for teacher, S for student. Left it as char so 
        /// we could add more user types if we ever needed to.
        public char userType { get; set; }

        /// Stores the ProfilePicture for the user. This will be a link to the image root.
        public string? picturePath { get; set; }


        /// <summary>
        /// Stores the salt for the user. This is used for password hashing.
        /// </summary>

      
        /// for password Hashing

        public string? Salt { get; set; }

        /// <summary>
        /// AmountPaid is the amount of money the user has paid for the course.
        /// </summary>
        public double? amountPaid { get; set; } = 0;

    }
}
