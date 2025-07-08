using CMS.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.EntityFrameworkCore
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
        public DbSet<BrandCategory> BrandCategories { get; set; }  // Add the junction table

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

            // Configure BrandCategory (Junction Table)
            modelBuilder.Entity<BrandCategory>(entity =>
            {
                // Set composite key
                entity.HasKey(cc => new { cc.BrandId, cc.CategoryId });
                entity.Property(e => e.AvailableStock).IsRequired();

                // Configure many-to-many relationship with Brand
                entity.HasOne(cc => cc.Brand)
                      .WithMany(c => c.BrandCategories)
                      .HasForeignKey(cc => cc.BrandId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configure many-to-many relationship with Category
                entity.HasOne(cc => cc.Category)
                      .WithMany(c => c.BrandCategories)
                      .HasForeignKey(cc => cc.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Category entity (updated for many-to-many)
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
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Image entity
            modelBuilder.Entity<Image>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.URL).IsRequired().HasMaxLength(500);

                // Configure relationship with composite foreign key
                entity.HasOne(i => i.BrandCategory)
                      .WithOne(cc => cc.Image)
                      .HasForeignKey<Image>(i => new { i.BrandId, i.CategoryId })
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}