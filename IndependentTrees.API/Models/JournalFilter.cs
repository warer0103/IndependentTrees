using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.Models
{
    public class JournalFilter : IValidatableObject
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
        
        public string Search { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (From.HasValue && To.HasValue && To.Value < From.Value)
                yield return new ValidationResult("Invalid date range.");
        }
    }
}
