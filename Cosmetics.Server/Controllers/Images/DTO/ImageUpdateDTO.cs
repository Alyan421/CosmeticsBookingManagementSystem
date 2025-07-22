using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Cosmetics.Server.Controllers.Images.DTO
{
    public class ImageUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }  // Changed from BrandId/CategoryId to ProductId

        // URL should be set by the server, not submitted from the client
        public string? URL { get; set; }
    }

    // Keep for backward compatibility with existing API endpoints
    public class ImageUpdateByBrandCategoryDTO
    {
        [FromForm(Name = "id")]
        public int Id { get; set; }

        [FromForm(Name = "brandId")]
        public int BrandId { get; set; }

        [FromForm(Name = "categoryId")]
        public int CategoryId { get; set; }

        // URL should be set by the server, not submitted from the client
        public string? URL { get; set; }
    }
}