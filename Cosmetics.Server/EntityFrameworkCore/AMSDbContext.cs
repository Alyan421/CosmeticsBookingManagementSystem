using Cosmetics.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Cosmetics.Server.EntityFrameworkCore
{
    public class AMSDbContext : DbContext
    {
        public AMSDbContext(DbContextOptions<AMSDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Make all table names lowercase for PostgreSQL compatibility
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToLower());

                // Convert column names to lowercase as well
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }

            // Configure User Entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id); // Primary Key

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .HasMaxLength(150);

                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(50);
            });

            // Configure Product Entity
            modelBuilder.Entity<Product>(entity =>
            {
                // Use Id as primary key instead of composite key
                entity.HasKey(p => p.Id);

                entity.Property(p => p.ProductName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.Description)
                      .HasMaxLength(1000);

                entity.Property(p => p.AvailableProduct).IsRequired();
                entity.Property(p => p.Price).IsRequired();

                // Configure many-to-one relationship with Brand
                entity.HasOne(p => p.Brand)
                      .WithMany(b => b.Products)
                      .HasForeignKey(p => p.BrandId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configure many-to-one relationship with Category
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Add index for better query performance
                entity.HasIndex(p => new { p.BrandId, p.CategoryId });
            });

            // Configure Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(50);
            });

            // Configure Brand entity
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Image entity
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.URL).IsRequired().HasMaxLength(500);

                // Configure relationship with single foreign key
                entity.HasOne(i => i.Product)
                      .WithOne(p => p.Image)
                      .HasForeignKey<Image>(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}