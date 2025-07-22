using System.ComponentModel.DataAnnotations;

namespace Cosmetics.Server.Controllers.Products.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public int AvailableProduct { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }  // Include image URL if available
    }

    public class ProductCreateDTO
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string ProductName { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available product cannot be negative")]
        public int AvailableProduct { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public double Price { get; set; }
    }

    public class ProductUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string ProductName { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Available product cannot be negative")]
        public int AvailableProduct { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public double Price { get; set; }
    }

    // New DTO for getting products by brand/category (backward compatibility)
    public class ProductByBrandCategoryDTO
    {
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public List<ProductSummaryDTO> Products { get; set; } = new List<ProductSummaryDTO>();
    }

    public class ProductSummaryDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public int AvailableProduct { get; set; }
        public double Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}