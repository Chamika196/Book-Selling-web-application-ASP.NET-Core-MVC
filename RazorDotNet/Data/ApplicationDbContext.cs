using Microsoft.EntityFrameworkCore;
using RazorDotNet.Models;

namespace RazorDotNet.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action1", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Action2", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Action3", DisplayOrder = 3 }
                );
        }
    }
}
