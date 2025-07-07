using Microsoft.AspNetCore.Mvc;
using CMS.Server.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CMS.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using CMS.Server.Controllers.Stock.DTO;
using AutoMapper;

namespace CMS.Server.Controllers.Stock
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly AMSDbContext _context;
        private readonly IMapper _mapper;

        public StockController(AMSDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStock()
        {
            try
            {
                var stocks = await _context.ClothColors
                    .Include(cc => cc.Cloth)
                    .Include(cc => cc.Color)
                    .Select(cc => new StockDTO
                    {
                        ClothId = cc.ClothId,
                        ColorId = cc.ColorId,
                        ClothName = cc.Cloth.Name,
                        ColorName = cc.Color.ColorName,
                        AvailableStock = cc.AvailableStock
                    })
                    .ToListAsync();

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("cloth/{clothId}")]
        public async Task<IActionResult> GetStockByCloth(int clothId)
        {
            try
            {
                var stocks = await _context.ClothColors
                    .Include(cc => cc.Cloth)
                    .Include(cc => cc.Color)
                    .Where(cc => cc.ClothId == clothId)
                    .Select(cc => new StockDTO
                    {
                        ClothId = cc.ClothId,
                        ColorId = cc.ColorId,
                        ClothName = cc.Cloth.Name,
                        ColorName = cc.Color.ColorName,
                        AvailableStock = cc.AvailableStock
                    })
                    .ToListAsync();

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("color/{colorId}")]
        public async Task<IActionResult> GetStockByColor(int colorId)
        {
            try
            {
                var stocks = await _context.ClothColors
                    .Include(cc => cc.Cloth)
                    .Include(cc => cc.Color)
                    .Where(cc => cc.ColorId == colorId)
                    .Select(cc => new StockDTO
                    {
                        ClothId = cc.ClothId,
                        ColorId = cc.ColorId,
                        ClothName = cc.Cloth.Name,
                        ColorName = cc.Color.ColorName,
                        AvailableStock = cc.AvailableStock
                    })
                    .ToListAsync();

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{clothId}/{colorId}")]
        public async Task<IActionResult> GetStock(int clothId, int colorId)
        {
            try
            {
                var clothColor = await _context.ClothColors
                    .Include(cc => cc.Cloth)
                    .Include(cc => cc.Color)
                    .FirstOrDefaultAsync(cc => cc.ClothId == clothId && cc.ColorId == colorId);

                if (clothColor == null)
                {
                    return NotFound($"Stock for cloth ID {clothId} and color ID {colorId} not found");
                }

                var stockDTO = new StockDTO
                {
                    ClothId = clothColor.ClothId,
                    ColorId = clothColor.ColorId,
                    ClothName = clothColor.Cloth.Name,
                    ColorName = clothColor.Color.ColorName,
                    AvailableStock = clothColor.AvailableStock
                };

                return Ok(stockDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStock(StockUpdateDTO stockUpdate)
        {
            try
            {
                // Validate input
                if (stockUpdate == null)
                {
                    return BadRequest("Stock update data is required.");
                }

                if (stockUpdate.AvailableStock < 0)
                {
                    return BadRequest("Stock cannot be negative.");
                }

                // Check if cloth and color exist
                var cloth = await _context.Cloths.FindAsync(stockUpdate.ClothId);
                if (cloth == null)
                {
                    return NotFound($"Cloth with ID {stockUpdate.ClothId} not found.");
                }

                var color = await _context.Colors.FindAsync(stockUpdate.ColorId);
                if (color == null)
                {
                    return NotFound($"Color with ID {stockUpdate.ColorId} not found.");
                }

                // Find the cloth-color combination
                var clothColor = await _context.ClothColors
                    .FirstOrDefaultAsync(cc =>
                        cc.ClothId == stockUpdate.ClothId &&
                        cc.ColorId == stockUpdate.ColorId);

                if (clothColor == null)
                {
                    // Create new cloth-color relationship
                    clothColor = new ClothColor
                    {
                        ClothId = stockUpdate.ClothId,
                        ColorId = stockUpdate.ColorId,
                        AvailableStock = stockUpdate.AvailableStock
                    };

                    await _context.ClothColors.AddAsync(clothColor);
                }
                else
                {
                    // Update existing stock
                    clothColor.AvailableStock = stockUpdate.AvailableStock;
                }

                await _context.SaveChangesAsync();

                // Create response DTO
                var stockDTO = new StockDTO
                {
                    ClothId = clothColor.ClothId,
                    ColorId = clothColor.ColorId,
                    ClothName = cloth.Name,
                    ColorName = color.ColorName,
                    AvailableStock = clothColor.AvailableStock
                };

                return Ok(stockDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkUpdateStock(List<StockUpdateDTO> stockUpdates)
        {
            try
            {
                if (stockUpdates == null || !stockUpdates.Any())
                {
                    return BadRequest("No stock updates provided.");
                }

                // Validate all stock updates
                foreach (var update in stockUpdates)
                {
                    if (update.AvailableStock < 0)
                    {
                        return BadRequest($"Stock cannot be negative for Cloth ID {update.ClothId} and Color ID {update.ColorId}.");
                    }
                }

                // Batch process all updates
                foreach (var update in stockUpdates)
                {
                    // Check if cloth and color exist (can be optimized with a batch query)
                    var cloth = await _context.Cloths.FindAsync(update.ClothId);
                    if (cloth == null) continue; // Skip invalid entries

                    var color = await _context.Colors.FindAsync(update.ColorId);
                    if (color == null) continue; // Skip invalid entries

                    var clothColor = await _context.ClothColors
                        .FirstOrDefaultAsync(cc =>
                            cc.ClothId == update.ClothId &&
                            cc.ColorId == update.ColorId);

                    if (clothColor == null)
                    {
                        // Create new relationship
                        clothColor = new ClothColor
                        {
                            ClothId = update.ClothId,
                            ColorId = update.ColorId,
                            AvailableStock = update.AvailableStock
                        };

                        await _context.ClothColors.AddAsync(clothColor);
                    }
                    else
                    {
                        // Update existing
                        clothColor.AvailableStock = update.AvailableStock;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Stock updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{clothId}/{colorId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveStock(int clothId, int colorId)
        {
            try
            {
                var clothColor = await _context.ClothColors
                    .FirstOrDefaultAsync(cc => cc.ClothId == clothId && cc.ColorId == colorId);

                if (clothColor == null)
                {
                    return NotFound($"Stock for Cloth ID {clothId} and Color ID {colorId} not found.");
                }

                // Before removing, check if there are any images associated with this cloth-color
                var associatedImages = await _context.Images
                    .Where(i => i.ClothId == clothId && i.ColorId == colorId)
                    .ToListAsync();

                if (associatedImages.Any())
                {
                    return BadRequest($"Cannot remove stock while there are {associatedImages.Count} images associated with this cloth-color combination. Delete the images first.");
                }

                _context.ClothColors.Remove(clothColor);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}