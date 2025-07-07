using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Colors.DTO;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using CMS.Server.Managers.Colors;

namespace CMS.Server.Controllers.Colors
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColorController : ControllerBase, IColorController
    {
        private readonly IColorManager _colorManager;
        private readonly IMapper _mapper;

        public ColorController(IColorManager colorManager, IMapper mapper)
        {
            _colorManager = colorManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColorGetDTO>>> GetAllColors()
        {
            var colors = await _colorManager.GetAllColorsAsync();
            return _mapper.Map<List<ColorGetDTO>>(colors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColorGetDTO>> GetColorById(int id)
        {
            var color = await _colorManager.GetColorByIdAsync(id);

            if (color == null)
            {
                return NotFound($"Color with ID {id} not found");
            }

            return _mapper.Map<ColorGetDTO>(color);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ColorGetDTO>> CreateColor(ColorCreateDTO colorCreateDTO)
        {
            var createdColor = await _colorManager.CreateColorAsync(colorCreateDTO);
            return _mapper.Map<ColorGetDTO>(createdColor);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ColorGetDTO>> UpdateColor(ColorUpdateDTO colorUpdateDTO)
        {
            var updatedColor = await _colorManager.UpdateColorAsync(colorUpdateDTO);

            if (updatedColor == null)
            {
                return NotFound($"Color with ID {colorUpdateDTO.Id} not found");
            }

            return _mapper.Map<ColorGetDTO>(updatedColor);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteColor(int id)
        {
            try
            {
                var result = await _colorManager.DeleteColorAsync(id);

                if (!result)
                {
                    return NotFound($"Color with ID {id} not found");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}