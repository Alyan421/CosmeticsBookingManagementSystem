namespace Cosmetics.Server.Models
{
    public class Image : BaseEntity<int>
    {
        public string URL { get; set; }

        public int ProductId { get; set; }

        // Navigation property
        public Product Product { get; set; }
    }
}