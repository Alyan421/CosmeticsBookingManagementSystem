using System.ComponentModel.DataAnnotations;
using Cosmetics.Server.Models;

namespace Cosmetics.Server.Controllers.Brands.DTO
{
    public class BrandCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}