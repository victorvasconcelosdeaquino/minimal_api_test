using Microsoft.EntityFrameworkCore;
using minimal_api_test.Entities;

namespace minimal_api_test.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product>? Products { get; set; }
    public DbSet<Category>? Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Category>().HasKey(c => c.Id);
        builder.Entity<Category>().Property(c => c.Name)
                                  .HasMaxLength(100)
                                  .IsRequired();

        builder.Entity<Product>().HasKey(c => c.Id);
        builder.Entity<Product>().Property(c => c.Name)
                                 .HasMaxLength(100)
                                 .IsRequired();
        builder.Entity<Product>().Property(c => c.Description)
                                 .HasMaxLength(150);
        builder.Entity<Product>().Property(c => c.Price)
                                 .HasPrecision(14, 2);

        builder.Entity<Product>()
            .HasOne<Category>(c => c.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(c => c.CategoryId);
    }
}
