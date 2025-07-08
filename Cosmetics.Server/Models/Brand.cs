using CMS.Server.Models;

public class Brand : BaseEntity<int>
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; }
}