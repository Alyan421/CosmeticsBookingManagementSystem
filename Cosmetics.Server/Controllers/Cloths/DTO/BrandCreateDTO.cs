using System.ComponentModel.DataAnnotations;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Brands.DTO
{
    public class BrandCreateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Description { get; set; }
    }

    // DTO for the BrandCategory entity to use within the BrandCreateDTO
    public class BrandCategoryDTO
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int AvailableStock { get; set; } = 0;
    }
}