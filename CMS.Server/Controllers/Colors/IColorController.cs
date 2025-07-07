using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Colors.DTO;

namespace CMS.Server.Controllers.Colors
{
    public interface IColorController
    {
        Task<ActionResult<IEnumerable<ColorGetDTO>>> GetAllColors();
        Task<ActionResult<ColorGetDTO>> GetColorById(int id);
        Task<ActionResult<ColorGetDTO>> CreateColor(ColorCreateDTO colorCreateDTO);
        Task<ActionResult<ColorGetDTO>> UpdateColor(ColorUpdateDTO colorUpdateDTO);
        Task<IActionResult> DeleteColor(int id);
    }
}