namespace Cosmetics.Server.Controllers.Images.DTO
{
    public class ImageGetDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }  // Changed from BrandId/CategoryId to ProductId
        public string URL { get; set; }

        // Product information for convenience
        public string ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public double Price { get; set; }
        public int AvailableProduct { get; set; }

        // Brand and Category information for backward compatibility
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
    }
}