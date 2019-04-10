using Microsoft.EntityFrameworkCore;
using MicroService.Model;

namespace MicroService.DBContexts
{
    public class ProductContext:DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
          new Category
          {
              ID = 1,
              Name = "Electronics",
              Des = "Electronic Items",
          },
          new Category
          {
              ID = 2,
              Name = "Clothes",
              Des = "Dresses",
          },
          new Category
          {
              ID = 3,
              Name = "Grocery",
              Des = "Grocery Items",
          }
      );
        }
    }
}
