using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.Models
{
    public class Node
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public Node[] Children { get; set; }
    }
}
