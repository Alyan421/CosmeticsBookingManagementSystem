using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Models
{
    public class ClothColor
    {
        public int ClothId { get; set; }
        public Cloth Cloth { get; set; }

        public int ColorId { get; set; }
        public Color Color { get; set; }
        public int AvailableStock { get; set; }
        public Image Image { get; set; }
    }
}