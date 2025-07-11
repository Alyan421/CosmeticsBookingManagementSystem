using System.ComponentModel.DataAnnotations;

namespace Cosmetics.Server.Controllers.Brands.DTO
{
    public class BrandUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}