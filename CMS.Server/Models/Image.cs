using CMS.Server.Models;

public class Image : BaseEntity<int>
{
    public string URL { get; set; }

    // Replace single foreign key with matching composite keys
    public int ClothId { get; set; }
    public int ColorId { get; set; }

    // Navigation property
    public ClothColor ClothColor { get; set; }
}