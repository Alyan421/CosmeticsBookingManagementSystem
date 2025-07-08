using CMS.Server.Models;

public class Brand : BaseEntity<int>
{
    public string Name { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
    public ICollection<BrandCategory> BrandCategories { get; set; }
}