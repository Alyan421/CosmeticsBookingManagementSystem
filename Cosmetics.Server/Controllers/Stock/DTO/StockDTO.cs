using System.ComponentModel.DataAnnotations;

namespace CMS.Server.Controllers.Stock.DTO
{
    public class StockDTO
    {
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int AvailableStock { get; set; }
    }

    public class StockUpdateDTO
    {
        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int AvailableStock { get; set; }
    }
}