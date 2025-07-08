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
                var stocks = await _context.BrandCategories
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .Select(cc => new StockDTO
                    {
                        BrandId = cc.BrandId,
                        CategoryId = cc.CategoryId,
                        BrandName = cc.Brand.Name,
                        CategoryName = cc.Category.CategoryName,
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

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetStockByBrand(int brandId)
        {
            try
            {
                var stocks = await _context.BrandCategories
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .Where(cc => cc.BrandId == brandId)
                    .Select(cc => new StockDTO
                    {
                        BrandId = cc.BrandId,
                        CategoryId = cc.CategoryId,
                        BrandName = cc.Brand.Name,
                        CategoryName = cc.Category.CategoryName,
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

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetStockByCategory(int categoryId)
        {
            try
            {
                var stocks = await _context.BrandCategories
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .Where(cc => cc.CategoryId == categoryId)
                    .Select(cc => new StockDTO
                    {
                        BrandId = cc.BrandId,
                        CategoryId = cc.CategoryId,
                        BrandName = cc.Brand.Name,
                        CategoryName = cc.Category.CategoryName,
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

        [HttpGet("{brandId}/{categoryId}")]
        public async Task<IActionResult> GetStock(int brandId, int categoryId)
        {
            try
            {
                var brandCategory = await _context.BrandCategories
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .FirstOrDefaultAsync(cc => cc.BrandId == brandId && cc.CategoryId == categoryId);

                if (brandCategory == null)
                {
                    return NotFound($"Stock for brand ID {brandId} and category ID {categoryId} not found");
                }

                var stockDTO = new StockDTO
                {
                    BrandId = brandCategory.BrandId,
                    CategoryId = brandCategory.CategoryId,
                    BrandName = brandCategory.Brand.Name,
                    CategoryName = brandCategory.Category.CategoryName,
                    AvailableStock = brandCategory.AvailableStock
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

                // Check if brand and category exist
                var brand = await _context.Brands.FindAsync(stockUpdate.BrandId);
                if (brand == null)
                {
                    return NotFound($"Brand with ID {stockUpdate.BrandId} not found.");
                }

                var category = await _context.Categories.FindAsync(stockUpdate.CategoryId);
                if (category == null)
                {
                    return NotFound($"Category with ID {stockUpdate.CategoryId} not found.");
                }

                // Find the brand-category combination
                var brandCategory = await _context.BrandCategories
                    .FirstOrDefaultAsync(cc =>
                        cc.BrandId == stockUpdate.BrandId &&
                        cc.CategoryId == stockUpdate.CategoryId);

                if (brandCategory == null)
                {
                    // Create new brand-category relationship
                    brandCategory = new BrandCategory
                    {
                        BrandId = stockUpdate.BrandId,
                        CategoryId = stockUpdate.CategoryId,
                        AvailableStock = stockUpdate.AvailableStock
                    };

                    await _context.BrandCategories.AddAsync(brandCategory);
                }
                else
                {
                    // Update existing stock
                    brandCategory.AvailableStock = stockUpdate.AvailableStock;
                }

                await _context.SaveChangesAsync();

                // Create response DTO
                var stockDTO = new StockDTO
                {
                    BrandId = brandCategory.BrandId,
                    CategoryId = brandCategory.CategoryId,
                    BrandName = brand.Name,
                    CategoryName = category.CategoryName,
                    AvailableStock = brandCategory.AvailableStock
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
                        return BadRequest($"Stock cannot be negative for Brand ID {update.BrandId} and Category ID {update.CategoryId}.");
                    }
                }

                // Batch process all updates
                foreach (var update in stockUpdates)
                {
                    // Check if brand and category exist (can be optimized with a batch query)
                    var brand = await _context.Brands.FindAsync(update.BrandId);
                    if (brand == null) continue; // Skip invalid entries

                    var category = await _context.Categories.FindAsync(update.CategoryId);
                    if (category == null) continue; // Skip invalid entries

                    var brandCategory = await _context.BrandCategories
                        .FirstOrDefaultAsync(cc =>
                            cc.BrandId == update.BrandId &&
                            cc.CategoryId == update.CategoryId);

                    if (brandCategory == null)
                    {
                        // Create new relationship
                        brandCategory = new BrandCategory
                        {
                            BrandId = update.BrandId,
                            CategoryId = update.CategoryId,
                            AvailableStock = update.AvailableStock
                        };

                        await _context.BrandCategories.AddAsync(brandCategory);
                    }
                    else
                    {
                        // Update existing
                        brandCategory.AvailableStock = update.AvailableStock;
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

        [HttpDelete("{brandId}/{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveStock(int brandId, int categoryId)
        {
            try
            {
                var brandCategory = await _context.BrandCategories
                    .FirstOrDefaultAsync(cc => cc.BrandId == brandId && cc.CategoryId == categoryId);

                if (brandCategory == null)
                {
                    return NotFound($"Stock for Brand ID {brandId} and Category ID {categoryId} not found.");
                }

                // Before removing, check if there are any images associated with this brand-category
                var associatedImages = await _context.Images
                    .Where(i => i.BrandId == brandId && i.CategoryId == categoryId)
                    .ToListAsync();

                if (associatedImages.Any())
                {
                    return BadRequest($"Cannot remove stock while there are {associatedImages.Count} images associated with this brand-category combination. Delete the images first.");
                }

                _context.BrandCategories.Remove(brandCategory);
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