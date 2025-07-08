using System.ComponentModel.DataAnnotations;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Brands.DTO
{
    public class BrandCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}