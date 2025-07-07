using AutoMapper;
using CMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Managers.Cloths;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Controllers.Cloths.DTO;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CMS.Server.Controllers.Cloths
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClothController : ControllerBase, IClothController
    {
        private readonly IClothManager _clothManager;
        private readonly IMapper _mapper;

        public ClothController(IClothManager clothManager, IMapper mapper)
        {
            _clothManager = clothManager;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(ClothCreateDTO clothCreateDTO)
        {
            try
            {
                if (clothCreateDTO == null)
                {
                    return BadRequest("Cloth data is required.");
                }

                // Map DTO to entity
                var cloth = _mapper.Map<Cloth>(clothCreateDTO);

                // Create cloth with relationships
                var createdCloth = await _clothManager.CreateClothAsync(cloth);

                if (createdCloth == null)
                {
                    return StatusCode(500, "An error occurred while creating the cloth.");
                }

                var clothDTO = _mapper.Map<ClothGetDTO>(createdCloth);
                return Ok(clothDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid cloth ID.");
                }

                var cloth = await _clothManager.GetClothByIdAsync(id);

                if (cloth == null)
                {
                    return NotFound($"Cloth with ID {id} not found.");
                }

                var clothDTO = _mapper.Map<ClothGetDTO>(cloth);
                return Ok(clothDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(ClothUpdateDTO clothUpdateDTO)
        {
            try
            {
                if (clothUpdateDTO == null || clothUpdateDTO.Id <= 0)
                {
                    return BadRequest("Valid cloth data is required.");
                }

                // Get existing cloth
                var existingCloth = await _clothManager.GetClothByIdAsync(clothUpdateDTO.Id);
                if (existingCloth == null)
                {
                    return NotFound($"Cloth with ID {clothUpdateDTO.Id} not found.");
                }

                // Update basic properties
                existingCloth.Name = clothUpdateDTO.Name;
                existingCloth.Price = clothUpdateDTO.Price;
                existingCloth.Description = clothUpdateDTO.Description;

                // MISSING LINE: Save the updated cloth
                await _clothManager.UpdateClothAsync(existingCloth);

                var clothDTO = _mapper.Map<ClothGetDTO>(existingCloth);
                return Ok(clothDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClothAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid cloth ID.");
                }

                var result = await _clothManager.DeleteClothAsync(id);
                if (!result)
                {
                    return NotFound($"Cloth with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var cloths = await _clothManager.GetAllClothsAsync();

                if (cloths == null || !cloths.Any())
                {
                    return NotFound("No cloths found.");
                }

                var clothDTOs = _mapper.Map<IEnumerable<ClothGetDTO>>(cloths);
                return Ok(clothDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}