using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Models
{
    public class Product
    {
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int AvailableProduct { get; set; }
        public double Price { get; set; }  // New Price property
        public Image Image { get; set; }
    }
}