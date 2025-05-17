using IndependentTrees.API.DataStorage.EF.Tables;
using Microsoft.EntityFrameworkCore;

namespace IndependentTrees.API.DataStorage.EF
{
    public class IndependentTreesContext : DbContext
    {
        public IndependentTreesContext(DbContextOptions options) :
            base(options)
        { }

        public DbSet<ExceptionJournal> ExceptionJournals { get; set; } = null!;

        public DbSet<TreeNode> TreeNodes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TreeNode>()
                .HasOne(n => n.Parent)
                .WithMany(n => n.Children)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
