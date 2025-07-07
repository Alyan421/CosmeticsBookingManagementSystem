using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Controllers.Cloths.DTO
{
    public class ClothUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }

        public string? Description { get; set; }
    }
}