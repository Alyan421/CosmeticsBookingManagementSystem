using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMS.Server.Controllers.Categories.DTO
{
    public class CategoryUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }
    }
}