using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMS.Server.Controllers.Categories.DTO
{
    public class CategoryCreateDTO
    {
        [Required]
        public string CategoryName { get; set; }
    }
}