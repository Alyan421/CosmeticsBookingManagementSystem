using System.ComponentModel.DataAnnotations;

namespace Cosmetics.Server.Controllers.Products.DTO
{
    public class ProductDTO
    {
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int AvailableProduct { get; set; }
        public double Price { get; set; }
    }

    public class ProductUpdateDTO
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Product cannot be negative")]
        public int AvailableProduct { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public double Price { get; set; }
    }

    public class ProductCreateDTO
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Product cannot be negative")]
        public int AvailableProduct { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public double Price { get; set; }
    }
}