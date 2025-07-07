namespace CMS.Server.Controllers.Images.DTO
{
    public class ImageGetDTO
    {
        public int Id { get; set; }
        public int ClothId { get; set; }
        public int ColorId { get; set; }
        public string URL { get; set; }
        public double Price { get; set; }
        public string ClothName { get; set; }
        public string ColorName { get; set; }
        public int AvailableStock { get; set; }
    }
}