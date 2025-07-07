using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMS.Server.Controllers.Colors.DTO
{
    public class ColorUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string ColorName { get; set; }
    }
}