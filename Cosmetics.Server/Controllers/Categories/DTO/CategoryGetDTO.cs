using System.Collections.Generic;

namespace Cosmetics.Server.Controllers.Categories.DTO
{
    public class CategoryGetDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        // List of brands this category is available for (through Product)
        public List<ProductInfoDTO> Products { get; set; } = new List<ProductInfoDTO>();
    }

    // DTO representing a Product junction
    public class ProductInfoDTO
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int AvailableProduct { get; set; }
    }
}