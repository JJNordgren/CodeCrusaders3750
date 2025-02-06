using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CS3750Assignment1.Models
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public User Student { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [ForeignKey("AssignmentId")]
        [ValidateNever]
        public Assignment Assignment { get; set; }
        
        public List<string>? FilePath { get; set; } = new List<string>();

        public string? TextSubmission { get; set; }

        [Required]
        public int TotalPoints { get; set; }

        public int? Grade { get; set; }

        [Required]
        public DateTime SubmissionTime { get; set; }
    }
}
