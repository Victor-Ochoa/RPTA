namespace RPTA.ProductApi.Data;

using Microsoft.EntityFrameworkCore;
using RPTA.ProductApi.Models;

public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(x => x.Id).IsClustered();
        modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(30).IsRequired();
        modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(500);
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(10,2);
    }
}