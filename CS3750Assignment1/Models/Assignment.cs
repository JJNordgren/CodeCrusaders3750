using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Assignment
    {
        /// <summary>
        /// Automatically generated Id for the assignment
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Title of the assignment
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Description of the assignment
        /// </summary>
        [Required]
        public string Description { get; set; }

        /// <summary>
        /// Due date for the assignment
        /// </summary>
        [Required]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Maximum possible points for the assignment
        /// </summary>
        [Required]
        [Range(1, 1000, ErrorMessage = "Max points must be between 1 and 1000.")]
        public int MaxPoints { get; set; }

        public int? ReceivedPoints { get; set; }

        /// <summary>
        /// Assignment type (e.g., H for Homework, P for Project, Q for Quiz)
        /// </summary>
        [Required]
        public char AssignmentType { get; set; }

        /// <summary>
        /// Foreign key linking the assignment to a course
        /// </summary>
        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        [ValidateNever]
        public Course Course { get; set; }

        /// <summary>
        /// Foreign key linking the instructor who created the assignment
        /// </summary>
        [Required]
        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        [ValidateNever]
        public User Instructor { get; set; }
    }
}
