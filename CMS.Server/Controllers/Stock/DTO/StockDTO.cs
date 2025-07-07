using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Controllers.Stock.DTO
{
    public class StockDTO
    {
        public int ClothId { get; set; }
        public int ColorId { get; set; }
        public string ClothName { get; set; }
        public string ColorName { get; set; }
        public int AvailableStock { get; set; }
    }

    public class StockUpdateDTO
    {
        [Required]
        public int ClothId { get; set; }

        [Required]
        public int ColorId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int AvailableStock { get; set; }
    }
}