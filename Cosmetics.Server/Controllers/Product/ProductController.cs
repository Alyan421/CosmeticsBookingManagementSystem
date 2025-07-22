using Microsoft.AspNetCore.Mvc;
using Cosmetics.Server.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cosmetics.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Cosmetics.Server.Controllers.Products.DTO;
using AutoMapper;

namespace Cosmetics.Server.Controllers.Products
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
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        BrandId = p.BrandId,
                        CategoryId = p.CategoryId,
                        BrandName = p.Brand.Name,
                        CategoryName = p.Category.CategoryName,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        AvailableProduct = p.AvailableProduct,
                        Price = p.Price,
                        ImageUrl = p.Image != null ? p.Image.URL : null
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found");
                }

                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    BrandName = product.Brand.Name,
                    CategoryName = product.Category.CategoryName,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    AvailableProduct = product.AvailableProduct,
                    Price = product.Price,
                    ImageUrl = product.Image?.URL
                };

                return Ok(productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetProductsByBrand(int brandId)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(brandId);
                if (brand == null)
                {
                    return NotFound($"Brand with ID {brandId} not found");
                }

                var products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .Where(p => p.BrandId == brandId)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        BrandId = p.BrandId,
                        CategoryId = p.CategoryId,
                        BrandName = p.Brand.Name,
                        CategoryName = p.Category.CategoryName,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        AvailableProduct = p.AvailableProduct,
                        Price = p.Price,
                        ImageUrl = p.Image != null ? p.Image.URL : null
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
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            try
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category == null)
                {
                    return NotFound($"Category with ID {categoryId} not found");
                }

                var products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .Where(p => p.CategoryId == categoryId)
                    .Select(p => new ProductDTO
                    {
                        Id = p.Id,
                        BrandId = p.BrandId,
                        CategoryId = p.CategoryId,
                        BrandName = p.Brand.Name,
                        CategoryName = p.Category.CategoryName,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        AvailableProduct = p.AvailableProduct,
                        Price = p.Price,
                        ImageUrl = p.Image != null ? p.Image.URL : null
                    })
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("brand/{brandId}/category/{categoryId}")]
        public async Task<IActionResult> GetProductsByBrandAndCategory(int brandId, int categoryId)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .Where(p => p.BrandId == brandId && p.CategoryId == categoryId)
                    .GroupBy(p => new { p.BrandId, p.CategoryId, p.Brand.Name, p.Category.CategoryName })
                    .Select(g => new ProductByBrandCategoryDTO
                    {
                        BrandId = g.Key.BrandId,
                        CategoryId = g.Key.CategoryId,
                        BrandName = g.Key.Name,
                        CategoryName = g.Key.CategoryName,
                        Products = g.Select(p => new ProductSummaryDTO
                        {
                            Id = p.Id,
                            ProductName = p.ProductName,
                            Description = p.Description,
                            AvailableProduct = p.AvailableProduct,
                            Price = p.Price,
                            ImageUrl = p.Image != null ? p.Image.URL : null
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                if (products == null || !products.Products.Any())
                {
                    return NotFound($"No products found for brand ID {brandId} and category ID {categoryId}");
                }

                return Ok(products);
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
                if (productCreate == null)
                {
                    return BadRequest("Product create data is required.");
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

                // Check if product name already exists for this brand-category combination
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p =>
                        p.BrandId == productCreate.BrandId &&
                        p.CategoryId == productCreate.CategoryId &&
                        p.ProductName.ToLower() == productCreate.ProductName.ToLower());

                if (existingProduct != null)
                {
                    return BadRequest($"A product with name '{productCreate.ProductName}' already exists for this brand-category combination.");
                }

                // Create new product
                var product = new Product
                {
                    BrandId = productCreate.BrandId,
                    CategoryId = productCreate.CategoryId,
                    ProductName = productCreate.ProductName,
                    Description = productCreate.Description,
                    AvailableProduct = productCreate.AvailableProduct,
                    Price = productCreate.Price
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                // Create response DTO
                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    BrandName = brand.Name,
                    CategoryName = category.CategoryName,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    AvailableProduct = product.AvailableProduct,
                    Price = product.Price,
                    ImageUrl = null
                };

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDTO productUpdate)
        {
            try
            {
                if (productUpdate == null)
                {
                    return BadRequest("Product update data is required.");
                }

                if (id != productUpdate.Id)
                {
                    return BadRequest("Product ID mismatch.");
                }

                var product = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
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

                // Check if product name already exists for this brand-category combination (excluding current product)
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p =>
                        p.Id != id &&
                        p.BrandId == productUpdate.BrandId &&
                        p.CategoryId == productUpdate.CategoryId &&
                        p.ProductName.ToLower() == productUpdate.ProductName.ToLower());

                if (existingProduct != null)
                {
                    return BadRequest($"A product with name '{productUpdate.ProductName}' already exists for this brand-category combination.");
                }

                // Update product properties
                product.BrandId = productUpdate.BrandId;
                product.CategoryId = productUpdate.CategoryId;
                product.ProductName = productUpdate.ProductName;
                product.Description = productUpdate.Description;
                product.AvailableProduct = productUpdate.AvailableProduct;
                product.Price = productUpdate.Price;

                await _context.SaveChangesAsync();

                // Create response DTO
                var productDTO = new ProductDTO
                {
                    Id = product.Id,
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    BrandName = brand.Name,
                    CategoryName = category.CategoryName,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    AvailableProduct = product.AvailableProduct,
                    Price = product.Price,
                    ImageUrl = product.Image?.URL
                };

                return Ok(productDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Image)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                // Check if there are any images associated with this product
                if (product.Image != null)
                {
                    return BadRequest("Cannot delete product while it has an associated image. Delete the image first.");
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

        // Backward compatibility endpoints
        [HttpGet("legacy/{brandId}/{categoryId}")]
        [Obsolete("Use GetProductsByBrandAndCategory instead")]
        public async Task<IActionResult> GetProductLegacy(int brandId, int categoryId)
        {
            return await GetProductsByBrandAndCategory(brandId, categoryId);
        }
    }
}