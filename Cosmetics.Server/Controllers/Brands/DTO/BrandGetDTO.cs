using System.ComponentModel.DataAnnotations;
using Cosmetics.Server.Controllers.Categories.DTO;
using System.Collections.Generic;

namespace Cosmetics.Server.Controllers.Brands.DTO
{
    public class BrandGetDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        // Add categories associated with this brand
        public List<CategoryInfoDTO> Categories { get; set; } = new List<CategoryInfoDTO>();
    }

    // Update CategoryInfoDTO to include product information
    public class CategoryInfoDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
    }
}