using System.Collections.Generic;

namespace CMS.Server.Controllers.Categories.DTO
{
    public class CategoryGetDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }

        // List of brands this category is available for (through BrandCategory)
        public List<BrandCategoryInfoDTO> BrandCategories { get; set; } = new List<BrandCategoryInfoDTO>();
    }

    // DTO representing a BrandCategory junction
    public class BrandCategoryInfoDTO
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int AvailableStock { get; set; }
    }
}