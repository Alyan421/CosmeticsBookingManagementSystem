using System.ComponentModel.DataAnnotations;
using CMS.Server.Controllers.Colors.DTO;
using System.Collections.Generic;

namespace CMS.Server.Controllers.Cloths.DTO
{
    public class ClothGetDTO
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }

        // Add colors associated with this cloth
        public List<ColorInfoDTO> Colors { get; set; } = new List<ColorInfoDTO>();
    }

    // Update ColorInfoDTO to include stock information
    public class ColorInfoDTO
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
    }
}