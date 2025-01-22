using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Course
    {
        public int Id { get; set; }

        public string Department { get; set; }

        public string Name { get; set; }

        public string ClassNumber { get; set; }

        public int CreditHours { get; set; }

        public int Capacity { get; set; }

        public string Location { get; set; }

        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public User Instructor { get; set; }
    }
}
