using System.ComponentModel.DataAnnotations;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Cloths.DTO
{
    public class ClothCreateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Description { get; set; }
    }

    // DTO for the ClothColor entity to use within the ClothCreateDTO
    public class ClothColorDTO
    {
        [Required]
        public int ColorId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative")]
        public int AvailableStock { get; set; } = 0;
    }
}