using Microsoft.AspNetCore.Mvc;
using CMS.Server.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CMS.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using CMS.Server.Controllers.Products.DTO;
using AutoMapper;

namespace CMS.Server.Controllers.Products
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AMSDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(AMSDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                var products = await _context.Products
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .Select(cc => new ProductDTO
                    {
                        BrandId = cc.BrandId,
                        CategoryId = cc.CategoryId,
                        BrandName = cc.Brand.Name,
                        CategoryName = cc.Category.CategoryName,
                        AvailableProduct = cc.AvailableProduct,
                        Price = cc.Price // Include Price in the selection
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetProductByBrand(int brandId)
        {
            try
            {
                var products = await _context.Products
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .Where(cc => cc.BrandId == brandId)
                    .Select(cc => new ProductDTO
                    {
                        BrandId = cc.BrandId,
                        CategoryId = cc.CategoryId,
                        BrandName = cc.Brand.Name,
                        CategoryName = cc.Category.CategoryName,
                        AvailableProduct = cc.AvailableProduct,
                        Price = cc.Price // Include Price in the selection
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductByCategory(int categoryId)
        {
            try
            {
                var products = await _context.Products
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .Where(cc => cc.CategoryId == categoryId)
                    .Select(cc => new ProductDTO
                    {
                        BrandId = cc.BrandId,
                        CategoryId = cc.CategoryId,
                        BrandName = cc.Brand.Name,
                        CategoryName = cc.Category.CategoryName,
                        AvailableProduct = cc.AvailableProduct,
                        Price = cc.Price // Include Price in the selection
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{brandId}/{categoryId}")]
        public async Task<IActionResult> GetProduct(int brandId, int categoryId)
        {
            try
            {
                var product = await _context.Products
                    .Include(cc => cc.Brand)
                    .Include(cc => cc.Category)
                    .FirstOrDefaultAsync(cc => cc.BrandId == brandId && cc.CategoryId == categoryId);

                if (product == null)
                {
                    return NotFound($"Product for brand ID {brandId} and category ID {categoryId} not found");
                }

                var productDTO = new ProductDTO
                {
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    BrandName = product.Brand.Name,
                    CategoryName = product.Category.CategoryName,
                    AvailableProduct = product.AvailableProduct,
                    Price = product.Price // Include Price in the DTO
                };

                return Ok(productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(ProductCreateDTO productCreate)
        {
            try
            {
                // Validate input
                if (productCreate == null)
                {
                    return BadRequest("Product create data is required.");
                }

                if (productCreate.AvailableProduct < 0)
                {
                    return BadRequest("Product cannot be negative.");
                }

                if (productCreate.Price < 0)
                {
                    return BadRequest("Price cannot be negative.");
                }

                // Check if brand and category exist
                var brand = await _context.Brands.FindAsync(productCreate.BrandId);
                if (brand == null)
                {
                    return NotFound($"Brand with ID {productCreate.BrandId} not found.");
                }

                var category = await _context.Categories.FindAsync(productCreate.CategoryId);
                if (category == null)
                {
                    return NotFound($"Category with ID {productCreate.CategoryId} not found.");
                }

                // Check if the combination already exists
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(cc =>
                        cc.BrandId == productCreate.BrandId &&
                        cc.CategoryId == productCreate.CategoryId);

                if (existingProduct != null)
                {
                    return BadRequest("This brand-category combination already exists.");
                }

                // Create new product
                var product = new CMS.Server.Models.Product
                {
                    BrandId = productCreate.BrandId,
                    CategoryId = productCreate.CategoryId,
                    AvailableProduct = productCreate.AvailableProduct,
                    Price = productCreate.Price
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                // Create response DTO
                var productDTO = new ProductDTO
                {
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    BrandName = brand.Name,
                    CategoryName = category.CategoryName,
                    AvailableProduct = product.AvailableProduct,
                    Price = product.Price
                };

                return CreatedAtAction(nameof(GetProduct),
                    new { brandId = product.BrandId, categoryId = product.CategoryId },
                    productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(ProductUpdateDTO productUpdate)
        {
            try
            {
                // Validate input
                if (productUpdate == null)
                {
                    return BadRequest("Product update data is required.");
                }

                if (productUpdate.AvailableProduct < 0)
                {
                    return BadRequest("Product cannot be negative.");
                }

                if (productUpdate.Price < 0)
                {
                    return BadRequest("Price cannot be negative.");
                }

                // Check if brand and category exist
                var brand = await _context.Brands.FindAsync(productUpdate.BrandId);
                if (brand == null)
                {
                    return NotFound($"Brand with ID {productUpdate.BrandId} not found.");
                }

                var category = await _context.Categories.FindAsync(productUpdate.CategoryId);
                if (category == null)
                {
                    return NotFound($"Category with ID {productUpdate.CategoryId} not found.");
                }

                // Find the brand-category combination
                var product = await _context.Products
                    .FirstOrDefaultAsync(cc =>
                        cc.BrandId == productUpdate.BrandId &&
                        cc.CategoryId == productUpdate.CategoryId);

                if (product == null)
                {
                    // Create new brand-category relationship
                    product = new CMS.Server.Models.Product
                    {
                        BrandId = productUpdate.BrandId,
                        CategoryId = productUpdate.CategoryId,
                        AvailableProduct = productUpdate.AvailableProduct,
                        Price = productUpdate.Price
                    };

                    await _context.Products.AddAsync(product);
                }
                else
                {
                    // Update existing product
                    product.AvailableProduct = productUpdate.AvailableProduct;
                    product.Price = productUpdate.Price;
                }

                await _context.SaveChangesAsync();

                // Create response DTO
                var productDTO = new ProductDTO
                {
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    BrandName = brand.Name,
                    CategoryName = category.CategoryName,
                    AvailableProduct = product.AvailableProduct,
                    Price = product.Price
                };

                return Ok(productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("bulk")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkUpdateProduct(List<ProductUpdateDTO> productUpdates)
        {
            try
            {
                if (productUpdates == null || !productUpdates.Any())
                {
                    return BadRequest("No product updates provided.");
                }

                // Validate all product updates
                foreach (var update in productUpdates)
                {
                    if (update.AvailableProduct < 0)
                    {
                        return BadRequest($"Product cannot be negative for Brand ID {update.BrandId} and Category ID {update.CategoryId}.");
                    }

                    if (update.Price < 0)
                    {
                        return BadRequest($"Price cannot be negative for Brand ID {update.BrandId} and Category ID {update.CategoryId}.");
                    }
                }

                // Batch process all updates
                foreach (var update in productUpdates)
                {
                    // Check if brand and category exist (can be optimized with a batch query)
                    var brand = await _context.Brands.FindAsync(update.BrandId);
                    if (brand == null) continue; // Skip invalid entries

                    var category = await _context.Categories.FindAsync(update.CategoryId);
                    if (category == null) continue; // Skip invalid entries

                    var product = await _context.Products
                        .FirstOrDefaultAsync(cc =>
                            cc.BrandId == update.BrandId &&
                            cc.CategoryId == update.CategoryId);

                    if (product == null)
                    {
                        // Create new relationship
                        product = new CMS.Server.Models.Product
                        {
                            BrandId = update.BrandId,
                            CategoryId = update.CategoryId,
                            AvailableProduct = update.AvailableProduct,
                            Price = update.Price
                        };

                        await _context.Products.AddAsync(product);
                    }
                    else
                    {
                        // Update existing
                        product.AvailableProduct = update.AvailableProduct;
                        product.Price = update.Price;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok("Products updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{brandId}/{categoryId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveProduct(int brandId, int categoryId)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(cc => cc.BrandId == brandId && cc.CategoryId == categoryId);

                if (product == null)
                {
                    return NotFound($"Product for Brand ID {brandId} and Category ID {categoryId} not found.");
                }

                // Before removing, check if there are any images associated with this brand-category
                var associatedImages = await _context.Images
                    .Where(i => i.BrandId == brandId && i.CategoryId == categoryId)
                    .ToListAsync();

                if (associatedImages.Any())
                {
                    return BadRequest($"Cannot remove product while there are {associatedImages.Count} images associated with this brand-category combination. Delete the images first.");
                }

                _context.Products.Remove(product);
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