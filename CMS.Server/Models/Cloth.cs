using CMS.Server.Models;

public class Cloth : BaseEntity<int>
{
    public string Name { get; set; }
    public double Price { get; set; }
    public string? Description { get; set; }
    public ICollection<ClothColor> ClothColors { get; set; }
}