using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Cosmetics.Server.Controllers.Images.DTO
{
    public class ImageCreateDTO
    {
        [Required]
        public int ProductId { get; set; }  // Changed from BrandId/CategoryId to ProductId

        [Required]
        public IFormFile ImageFile { get; set; }
    }

    // Keep for backward compatibility with existing API endpoints
    public class ImageCreateByBrandCategoryDTO
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public IFormFile ImageFile { get; set; }
    }
}