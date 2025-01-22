using System.ComponentModel.DataAnnotations;

namespace CS3750Assignment1.Attributes
{
    public class MinAgeAttribute : ValidationAttribute
    {
        private readonly int _minAge;

        public MinAgeAttribute(int minAge)
        {
            _minAge = minAge;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime birthDate)
            {
                int age = DateTime.Today.Year - birthDate.Year;
                if (birthDate > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                if (age < _minAge)
                {
                    return new ValidationResult($"You must be at least {_minAge} years old.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
