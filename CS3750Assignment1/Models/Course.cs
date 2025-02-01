using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Course
    {
        /// <summary>
        /// automatically generated class Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// stores the name of the department
        /// </summary>
        [Required]
        public string Department { get; set; }

        /// <summary>
        /// Stores the name of the class
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// stores the class number
        /// </summary>
        [Required]
        [RegularExpression(@"^([0-9])([0-9])([0-9])([0-9])$", ErrorMessage = "Course Number must be only 4 digits.")]
        public string ClassNumber { get; set; }

        /// <summary>
        /// stores the amount of credit hours the class is worth
        /// </summary>
        [Required]
        [Range(1,10, ErrorMessage = "Credit hours must be between 1 and 10.")]
        public int CreditHours { get; set; }

        /// <summary>
        /// stores the maximum capacity for the class
        /// </summary>
        [Required]
        [Range(1,100, ErrorMessage = "Class capacity must be between 1 and 100.")]
        public int Capacity { get; set; }

        /// <summary>
        /// stores the building and room number for the class to meet at
        /// </summary>
        [Required]
        public string Location { get; set; }

        /// <summary>
        /// A list of strings to store the days of the week that the class will be meeting
        /// </summary>
        [Required(ErrorMessage = "Please select at least one meetintg day.")]
        public List<string> MeetingDays { get; set; }

        /// <summary>
        /// Stores the start time for the class
        /// </summary>
        [Required]
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// Stores the end time for the class
        /// </summary>
        [Required]
        public TimeSpan EndTime { get; set; }

        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

        /// <summary>
        /// stores the instructor's ID
        /// </summary>
        public int InstructorId { get; set; }

        /// <summary>
        /// declare instructorId as a foreign Key that references the user table, not sure I have done
        /// this right
        /// </summary>
        [ForeignKey("InstructorId")]
        [ValidateNever]
        public User Instructor { get; set; }
    }
}
