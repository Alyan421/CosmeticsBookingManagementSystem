using System.ComponentModel.DataAnnotations;

namespace Cosmetics.Server.Models
{
    public class Product : BaseEntity<int>
    {
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Required]
        public string ProductName { get; set; }

        public string? Description { get; set; }

        public int AvailableProduct { get; set; }
        public double Price { get; set; }
        public Image? Image { get; set; }
    }
}