using System.Collections.Generic;

namespace CMS.Server.Controllers.Colors.DTO
{
    public class ColorGetDTO
    {
        public int Id { get; set; }
        public string ColorName { get; set; }

        // List of cloths this color is available for (through ClothColor)
        public List<ClothColorInfoDTO> ClothColors { get; set; } = new List<ClothColorInfoDTO>();
    }

    // DTO representing a ClothColor junction
    public class ClothColorInfoDTO
    {
        public int ClothId { get; set; }
        public string ClothName { get; set; }
        public int AvailableStock { get; set; }
    }
}