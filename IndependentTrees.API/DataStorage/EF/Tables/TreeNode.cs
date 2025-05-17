using System.ComponentModel.DataAnnotations;

namespace IndependentTrees.API.DataStorage.EF.Tables
{
    public class TreeNode
    {
        public int Id { get; set; }

        [StringLength(64)]
        public string Name { get; set; }

        public int? ParentId { get; set; }  
        
        public int? TreeId { get; set; }
        
        public TreeNode Parent { get; set; }

        public TreeNode Tree { get; set; }

        public ICollection<TreeNode> Children { get; set; }
    }
}
