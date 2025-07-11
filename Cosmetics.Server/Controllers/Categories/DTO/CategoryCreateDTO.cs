using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Cosmetics.Server.Controllers.Categories.DTO
{
    public class CategoryCreateDTO
    {
        [Required]
        public string CategoryName { get; set; }
    }
}