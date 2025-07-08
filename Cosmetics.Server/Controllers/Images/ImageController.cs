using AutoMapper;
using CMS.Server.Models;
using Microsoft.AspNetCore.Mvc;
using CMS.Server.Managers.Images;
using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Services;
using CMS.Server.Controllers.Images.DTO;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CMS.Server.EntityFrameworkCore;
using AutoMapper.Configuration.Annotations;

namespace CMS.Server.Controllers.Images
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
            var result = await _manager.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("by-brand-category")]
        public async Task<IActionResult> GetByBrandCategory(int brandId, int categoryId)
        {
            var result = await _manager.GetByBrandCategoryIdAsync(brandId, categoryId);
            return Ok(result);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var result = await _manager.GetByCategoryIdAsync(categoryId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _manager.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("upload")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload([FromForm] ImageCreateDTO dto)
        {
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                return BadRequest("Image is required");

            // Check if the brand-category combination exists
            var brandCategory = await _context.BrandCategories
                .FirstOrDefaultAsync(cc => cc.BrandId == dto.BrandId && cc.CategoryId == dto.CategoryId);

            if (brandCategory == null)
            {
                // Create the brand-category relationship if it doesn't exist
                brandCategory = new BrandCategory
                {
                    BrandId = dto.BrandId,
                    CategoryId = dto.CategoryId,
                    AvailableStock = 0 // Default stock
                };

                _context.BrandCategories.Add(brandCategory);
                await _context.SaveChangesAsync();
            }

            var result = await _manager.CreateAsync(dto, dto.ImageFile);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id,
    [FromForm] int brandId,
    [FromForm] int categoryId,
    [FromForm] IFormFile imagefile = null)
        {
            var dto = new ImageUpdateDTO
            {
                Id = id,
                BrandId = brandId,
                CategoryId = categoryId
            };

            // Check if the brand-category combination exists
            var brandCategory = await _context.BrandCategories
            .FirstOrDefaultAsync(cc => cc.BrandId == dto.BrandId && cc.CategoryId == dto.CategoryId);

            if (brandCategory == null)
            {
                // Create the brand-category relationship if it doesn't exist
                brandCategory = new BrandCategory
                {
                    BrandId = dto.BrandId,
                    CategoryId = dto.CategoryId,
                    AvailableStock = 0 // Default stock
                };

                _context.BrandCategories.Add(brandCategory);
                await _context.SaveChangesAsync();
            }

            await _manager.UpdateAsync(dto, imagefile);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _manager.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("filter-by-brand-name/{brandName}")]
        public async Task<IActionResult> FilterByBrandName(string brandName)
        {
            var images = await _manager.FilterByBrandNameAsync(brandName);
            return Ok(images);
        }

        [HttpGet("filter-by-category/{categoryName}")]
        public async Task<IActionResult> FilterByCategory(string categoryName)
        {
            var images = await _manager.FilterByCategoryAsync(categoryName);
            return Ok(images);
        }

        [HttpGet("by-brand/{brandId}")]
        public async Task<IActionResult> GetImagesByBrandId(int brandId)
        {
            try
            {
                // Get all brand-category combinations for this brand
                var brandCategories = await _context.BrandCategories
                    .Where(cc => cc.BrandId == brandId)
                    .ToListAsync();

                if (!brandCategories.Any())
                {
                    return Ok(new List<ImageGetDTO>());
                }

                // Get images for these brand-category combinations
                var images = new List<ImageGetDTO>();
                foreach (var brandCategory in brandCategories)
                {
                    var brandCategoryImages = await _manager.GetByBrandCategoryIdAsync(brandCategory.BrandId, brandCategory.CategoryId);
                    if (brandCategoryImages != null && brandCategoryImages.Any())
                    {
                        images.AddRange(brandCategoryImages);
                    }
                }

                return Ok(images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}