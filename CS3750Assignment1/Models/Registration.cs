using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Registration
    {
        /// <summary>
        /// Automatically generated Id for each registration entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key for the User (Student)
        /// </summary>
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public User Student { get; set; }

        /// <summary>
        /// Foreign key for the Course
        /// </summary>
        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }
    }
}
