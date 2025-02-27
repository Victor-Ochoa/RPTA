namespace RPTA.ProductApi.Data;

using Microsoft.EntityFrameworkCore;
using RPTA.ProductApi.Models;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed some initial data
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Basic Product",
                Description = "A simple product for demonstration",
                Price = 19.99m,
                Stock = 100,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Premium Product",
                Description = "A premium product with advanced features",
                Price = 49.99m,
                Stock = 50,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}