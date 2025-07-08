namespace CMS.Server.Models
{
    public class Category : BaseEntity<int>
    {
        public string CategoryName { get; set; }
        public ICollection<BrandCategory> BrandCategories { get; set; }
    }
}
