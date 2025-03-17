using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string? CourseName { get; set; } //ex cs3750
        public string? AssignmentName { get; set; } //ex assignment 1

        public string? Message { get; set; } //graded or created

        /// <summary>
        /// Foreign key for the User (Student)
        /// </summary>
        [Required]
        public int UserId { get; set; } //student to sent notification to

        [ForeignKey("UserId")]
        [ValidateNever]
        public User Student { get; set; }

        /// <summary>
        /// Foreign key for the Course
        /// </summary>
        [Required]
        public int CourseId { get; set; } //course the notification is about

        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }
        public bool Deleted { get; set; } //not actually used in the project, just delete the notification when closed
        //public int Id { get; set; }
        //public string? CourseName { get; set; }
        //public string? AssignmentName { get; set; }

        //public string? Message { get; set; }

        ///// <summary>
        ///// Foreign key for the User (Student)
        ///// </summary>
        //[Required]
        //public int UserId { get; set; }

        //[ForeignKey("UserId")]
        //[ValidateNever]
        //public User Student { get; set; }

        ///// <summary>
        ///// Foreign key for the Course
        ///// </summary>
        //[Required]
        //public int CourseId { get; set; }

        //[ForeignKey("CourseId")]
        //[ValidateNever]
        //public Course Course { get; set; }
        //public bool Deleted { get; set; }
    }
}
