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
        public DbSet<Color> Colors { get; set; }
        public DbSet<Cloth> Cloths { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<ClothColor> ClothColors { get; set; }  // Add the junction table

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

            // Configure ClothColor (Junction Table)
            modelBuilder.Entity<ClothColor>(entity =>
            {
                // Set composite key
                entity.HasKey(cc => new { cc.ClothId, cc.ColorId });
                entity.Property(e => e.AvailableStock).IsRequired();

                // Configure many-to-many relationship with Cloth
                entity.HasOne(cc => cc.Cloth)
                      .WithMany(c => c.ClothColors)
                      .HasForeignKey(cc => cc.ClothId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Configure many-to-many relationship with Color
                entity.HasOne(cc => cc.Color)
                      .WithMany(c => c.ClothColors)
                      .HasForeignKey(cc => cc.ColorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Color entity (updated for many-to-many)
            modelBuilder.Entity<Color>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ColorName).IsRequired().HasMaxLength(50);
            });

            // Configure Cloth entity
            modelBuilder.Entity<Cloth>(entity =>
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
                entity.HasOne(i => i.ClothColor)
                      .WithOne(cc => cc.Image)
                      .HasForeignKey<Image>(i => new { i.ClothId, i.ColorId })
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}