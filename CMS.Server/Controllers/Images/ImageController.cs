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

        [HttpGet("by-cloth-color")]
        public async Task<IActionResult> GetByClothColor(int clothId, int colorId)
        {
            var result = await _manager.GetByClothColorIdAsync(clothId, colorId);
            return Ok(result);
        }

        [HttpGet("by-color/{colorId}")]
        public async Task<IActionResult> GetByColor(int colorId)
        {
            var result = await _manager.GetByColorIdAsync(colorId);
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

            // Check if the cloth-color combination exists
            var clothColor = await _context.ClothColors
                .FirstOrDefaultAsync(cc => cc.ClothId == dto.ClothId && cc.ColorId == dto.ColorId);

            if (clothColor == null)
            {
                // Create the cloth-color relationship if it doesn't exist
                clothColor = new ClothColor
                {
                    ClothId = dto.ClothId,
                    ColorId = dto.ColorId,
                    AvailableStock = 0 // Default stock
                };

                _context.ClothColors.Add(clothColor);
                await _context.SaveChangesAsync();
            }

            var result = await _manager.CreateAsync(dto, dto.ImageFile);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id,
    [FromForm] int clothId,
    [FromForm] int colorId,
    [FromForm] IFormFile imagefile = null)
        {
            var dto = new ImageUpdateDTO
            {
                Id = id,
                ClothId = clothId,
                ColorId = colorId
            };

            // Check if the cloth-color combination exists
            var clothColor = await _context.ClothColors
            .FirstOrDefaultAsync(cc => cc.ClothId == dto.ClothId && cc.ColorId == dto.ColorId);

            if (clothColor == null)
            {
                // Create the cloth-color relationship if it doesn't exist
                clothColor = new ClothColor
                {
                    ClothId = dto.ClothId,
                    ColorId = dto.ColorId,
                    AvailableStock = 0 // Default stock
                };

                _context.ClothColors.Add(clothColor);
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

        [HttpGet("filter-by-cloth-name/{clothName}")]
        public async Task<IActionResult> FilterByClothName(string clothName)
        {
            var images = await _manager.FilterByClothNameAsync(clothName);
            return Ok(images);
        }

        [HttpGet("filter-by-color/{colorName}")]
        public async Task<IActionResult> FilterByColor(string colorName)
        {
            var images = await _manager.FilterByColorAsync(colorName);
            return Ok(images);
        }

        [HttpGet("by-cloth/{clothId}")]
        public async Task<IActionResult> GetImagesByClothId(int clothId)
        {
            try
            {
                // Get all cloth-color combinations for this cloth
                var clothColors = await _context.ClothColors
                    .Where(cc => cc.ClothId == clothId)
                    .ToListAsync();

                if (!clothColors.Any())
                {
                    return Ok(new List<ImageGetDTO>());
                }

                // Get images for these cloth-color combinations
                var images = new List<ImageGetDTO>();
                foreach (var clothColor in clothColors)
                {
                    var clothColorImages = await _manager.GetByClothColorIdAsync(clothColor.ClothId, clothColor.ColorId);
                    if (clothColorImages != null && clothColorImages.Any())
                    {
                        images.AddRange(clothColorImages);
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