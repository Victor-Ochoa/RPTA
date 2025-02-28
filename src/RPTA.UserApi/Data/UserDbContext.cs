namespace RPTA.UserApi.Data;

using Microsoft.EntityFrameworkCore;
using RPTA.UserApi.Models;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(x => x.Id).IsClustered();
        modelBuilder.Entity<User>().Property(p => p.FirstName).HasMaxLength(30).IsRequired();
        modelBuilder.Entity<User>().Property(p => p.LastName).HasMaxLength(100);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().Property(p => p.Email).HasMaxLength(200).IsRequired();
    }
}