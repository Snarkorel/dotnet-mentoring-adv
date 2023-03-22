using CatalogService.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data.Database
{
    public class CatalogContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=CatalogServiceDB;Trusted_Connection=True;");
        }
    }
}
