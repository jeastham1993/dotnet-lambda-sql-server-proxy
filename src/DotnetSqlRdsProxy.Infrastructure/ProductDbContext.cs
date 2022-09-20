using DotnetSqlRdsProxy.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetSqlRdsProxy.Infrastructure;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Product { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
        modelBuilder.Entity<Product>().HasIndex(p => p.Name).IsUnique();
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("numeric(18,6)");
    }
}