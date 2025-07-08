using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Models
{
    public class BrandCategory
    {
        public int BrandId { get; set; }
        public Brand Brand { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int AvailableStock { get; set; }
        public Image Image { get; set; }
    }
}