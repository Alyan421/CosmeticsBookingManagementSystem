using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CMS.Server.Controllers.Colors.DTO
{
    public class ColorCreateDTO
    {
        [Required]
        public string ColorName { get; set; }
    }
}