using AutoMapper;
using Cosmetics.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Cosmetics.Server.Managers.Images;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmetics.Server.Services;
using Cosmetics.Server.Controllers.Images.DTO;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Cosmetics.Server.EntityFrameworkCore;
using AutoMapper.Configuration.Annotations;
using Microsoft.OpenApi.Validations.Rules;

namespace Cosmetics.Server.Controllers.Images
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase, IImageController
    {
        private readonly IImageManager _manager;
        private readonly AMSDbContext _context;

        public ImageController(IImageManager manager, AMSDbContext context)
        {
            _manager = manager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _manager.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _manager.GetByIdAsync(id);
                if (result == null) return NotFound($"Image with ID {id} not found");
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            try
            {
                var result = await _manager.GetByProductIdAsync(productId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            try
            {
                var result = await _manager.GetByCategoryIdAsync(categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(int brandId)
        {
            try
            {
                var result = await _manager.GetByBrandIdAsync(brandId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload([FromForm] ImageCreateDTO dto)
        {
            try
            {
                if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                    return BadRequest("Image file is required");

                // Validate that the product exists
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID {dto.ProductId} not found");
                }

                var result = await _manager.CreateAsync(dto, dto.ImageFile);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromForm] ImageUpdateDTO dto, [FromForm] IFormFile imageFile = null)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest("Image ID mismatch");

                // Validate that the product exists
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID {dto.ProductId} not found");
                }

                await _manager.UpdateAsync(dto, imageFile);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _manager.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filter-by-brand-name/{brandName}")]
        public async Task<IActionResult> FilterByBrandName(string brandName)
        {
            try
            {
                var images = await _manager.FilterByBrandNameAsync(brandName);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("filter-by-category/{categoryName}")]
        public async Task<IActionResult> FilterByCategory(string categoryName)
        {
            try
            {
                var images = await _manager.FilterByCategoryAsync(categoryName);
                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Backward compatibility endpoints
        [HttpGet("by-brand-category")]
        [Obsolete("Use GetByProductId instead")]
        public async Task<IActionResult> GetByBrandCategory(int brandId, int categoryId)
        {
            try
            {
                var result = await _manager.GetByProductIdAsync(brandId, categoryId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("upload-by-brand-category")]
        [Authorize(Roles = "Admin")]
        [Obsolete("Use Upload with ProductId instead")]
        public async Task<IActionResult> UploadByBrandCategory([FromForm] ImageCreateByBrandCategoryDTO dto)
        {
            try
            {
                if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                    return BadRequest("Image file is required");

                // Check if the brand and category exist
                var brand = await _context.Brands.FindAsync(dto.BrandId);
                if (brand == null)
                {
                    return NotFound($"Brand with ID {dto.BrandId} not found");
                }

                var category = await _context.Categories.FindAsync(dto.CategoryId);
                if (category == null)
                {
                    return NotFound($"Category with ID {dto.CategoryId} not found");
                }

                var result = await _manager.CreateByBrandCategoryAsync(dto, dto.ImageFile);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update-by-brand-category/{id}")]
        [Authorize(Roles = "Admin")]
        [Obsolete("Use Update with ProductId instead")]
        public async Task<IActionResult> UpdateByBrandCategory(int id,
            [FromForm] int brandId,
            [FromForm] int categoryId,
            [FromForm] IFormFile imageFile = null)
        {
            try
            {
                // Find the product with the given brand and category
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.BrandId == brandId && p.CategoryId == categoryId);

                if (product == null)
                {
                    return NotFound($"No product found for brand ID {brandId} and category ID {categoryId}");
                }

                var dto = new ImageUpdateDTO
                {
                    Id = id,
                    ProductId = product.Id
                };

                await _manager.UpdateAsync(dto, imageFile);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}