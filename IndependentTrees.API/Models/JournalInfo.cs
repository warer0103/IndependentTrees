using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.Models
{
    public class JournalInfo
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }
    }
}
