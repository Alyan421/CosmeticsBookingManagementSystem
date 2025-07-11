using System.ComponentModel.DataAnnotations;

namespace Cosmetics.Server.Controllers.Images.DTO
{
    public class ImageCreateDTO
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IFormFile ImageFile { get; set; }
    }
}