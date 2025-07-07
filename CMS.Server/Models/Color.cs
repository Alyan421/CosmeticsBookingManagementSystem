namespace CMS.Server.Models
{
    public class Color : BaseEntity<int>
    {
        public string ColorName { get; set; }
        public ICollection<ClothColor> ClothColors { get; set; }
    }
}
