namespace CMS.Server.Controllers.Images.DTO
{
    public class ImageGetDTO
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string URL { get; set; }
        public double Price { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int AvailableProduct { get; set; }
    }
}