using AutoMapper;
using CMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Managers.Brands;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Controllers.Brands.DTO;
using System;
using Microsoft.AspNetCore.Authorization;

namespace CMS.Server.Controllers.Brands
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase, IBrandController
    {
        private readonly IBrandManager _brandManager;
        private readonly IMapper _mapper;

        public BrandController(IBrandManager brandManager, IMapper mapper)
        {
            _brandManager = brandManager;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync(BrandCreateDTO brandCreateDTO)
        {
            try
            {
                if (brandCreateDTO == null)
                {
                    return BadRequest("Brand data is required.");
                }

                // Map DTO to entity
                var brand = _mapper.Map<Brand>(brandCreateDTO);

                // Create brand with relationships
                var createdBrand = await _brandManager.CreateBrandAsync(brand);

                if (createdBrand == null)
                {
                    return StatusCode(500, "An error occurred while creating the brand.");
                }

                var brandDTO = _mapper.Map<BrandGetDTO>(createdBrand);
                return Ok(brandDTO);
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
                    return BadRequest("Invalid brand ID.");
                }

                var brand = await _brandManager.GetBrandByIdAsync(id);

                if (brand == null)
                {
                    return NotFound($"Brand with ID {id} not found.");
                }

                var brandDTO = _mapper.Map<BrandGetDTO>(brand);
                return Ok(brandDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAsync(BrandUpdateDTO brandUpdateDTO)
        {
            try
            {
                if (brandUpdateDTO == null || brandUpdateDTO.Id <= 0)
                {
                    return BadRequest("Valid brand data is required.");
                }

                // Get existing brand
                var existingBrand = await _brandManager.GetBrandByIdAsync(brandUpdateDTO.Id);
                if (existingBrand == null)
                {
                    return NotFound($"Brand with ID {brandUpdateDTO.Id} not found.");
                }

                // Update basic properties
                existingBrand.Name = brandUpdateDTO.Name;
                existingBrand.Price = brandUpdateDTO.Price;
                existingBrand.Description = brandUpdateDTO.Description;

                // MISSING LINE: Save the updated brand
                await _brandManager.UpdateBrandAsync(existingBrand);

                var brandDTO = _mapper.Map<BrandGetDTO>(existingBrand);
                return Ok(brandDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrandAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid brand ID.");
                }

                var result = await _brandManager.DeleteBrandAsync(id);
                if (!result)
                {
                    return NotFound($"Brand with ID {id} not found.");
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
                var brands = await _brandManager.GetAllBrandsAsync();

                if (brands == null || !brands.Any())
                {
                    return NotFound("No brands found.");
                }

                var brandDTOs = _mapper.Map<IEnumerable<BrandGetDTO>>(brands);
                return Ok(brandDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}