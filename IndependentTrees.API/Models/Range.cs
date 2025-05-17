using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.Models
{
    public class Range<T>
    {
        [Required]
        public int Skip { get; set; }

        [Required]
        public int Count { get; set; }

        [Required]
        public T[] Items { get; set; }
    }
}
