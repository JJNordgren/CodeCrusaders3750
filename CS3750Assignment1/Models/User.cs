using CS3750Assignment1.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CS3750Assignment1.Models
{
    public class User
    {
        /// <summary>
        /// create the auto generated Id field for the database items
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// create the email variable for the database
        /// </summary>
        public string email {  get; set; }
        /// <summary>
        /// create the first name variable for the database
        /// </summary>
        public string firstName { get; set; }
        /// <summary>
        /// create the last name variable for the database
        /// </summary>
        public string lastName { get; set; }

        /// <summary>
        /// create the password variable for the database
        /// </summary>
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string password { get; set; }
        /*
        public string
        */
        [Required]
        [MinAge(16, ErrorMessage = "You must be at least 16 years old.")]
        public DateTime BirthDate { get; set; }
        /// <summary>
        /// Stores a Char for user type, T for teacher, S for student. Left it as char so 
        /// we could add more user types if we ever needed to.
        /// </summary>
        public char userType { get; set; }

        /// <summary>
        /// Stores the ProfilePicture for the user. This will be a link to the image root.
        /// </summary>
        public string? picturePath { get; set; }
    }
}
