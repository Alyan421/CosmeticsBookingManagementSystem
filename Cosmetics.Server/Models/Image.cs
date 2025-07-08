using CMS.Server.Models;

public class Image : BaseEntity<int>
{
    public string URL { get; set; }

    // Replace single foreign key with matching composite keys
    public int BrandId { get; set; }
    public int CategoryId { get; set; }

    // Navigation property
    public BrandCategory BrandCategory { get; set; }
}