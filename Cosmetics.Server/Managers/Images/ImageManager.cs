using Cosmetics.Server.Models;
using Cosmetics.Server.Repository;
using Cosmetics.Server.Controllers.Images.DTO;
using Cosmetics.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Cosmetics.Server.EntityFrameworkCore;

namespace Cosmetics.Server.Managers.Images
{
    public class ImageManager : IImageManager
    {
        private readonly IGenericRepository<Image> _imageRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IImageStorageService _imageStorage;
        private readonly AMSDbContext _context;

        public ImageManager(
            IGenericRepository<Image> imageRepository,
            IGenericRepository<Product> productRepository,
            IImageStorageService imageStorage,
            AMSDbContext context)
        {
            _imageRepository = imageRepository;
            _productRepository = productRepository;
            _imageStorage = imageStorage;
            _context = context;
        }

        public async Task<List<ImageGetDTO>> GetAllAsync()
        {
            try
            {
                var images = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .ToListAsync();

                return images.Select(i => new ImageGetDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    URL = i.URL,
                    ProductName = i.Product?.ProductName ?? "Unknown",
                    ProductDescription = i.Product?.Description,
                    Price = i.Product?.Price ?? 0,
                    AvailableProduct = i.Product?.AvailableProduct ?? 0,
                    BrandId = i.Product?.BrandId ?? 0,
                    CategoryId = i.Product?.CategoryId ?? 0,
                    BrandName = i.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = i.Product?.Category?.CategoryName ?? "Unknown"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all images", ex);
            }
        }

        public async Task<ImageGetDTO> GetByIdAsync(int id)
        {
            try
            {
                var image = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (image == null) return null;

                return new ImageGetDTO
                {
                    Id = image.Id,
                    ProductId = image.ProductId,
                    URL = image.URL,
                    ProductName = image.Product?.ProductName ?? "Unknown",
                    ProductDescription = image.Product?.Description,
                    Price = image.Product?.Price ?? 0,
                    AvailableProduct = image.Product?.AvailableProduct ?? 0,
                    BrandId = image.Product?.BrandId ?? 0,
                    CategoryId = image.Product?.CategoryId ?? 0,
                    BrandName = image.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = image.Product?.Category?.CategoryName ?? "Unknown"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving image with ID {id}", ex);
            }
        }

        public async Task<List<ImageGetDTO>> GetByProductIdAsync(int productId)
        {
            try
            {
                var images = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .Where(i => i.ProductId == productId)
                    .ToListAsync();

                return images.Select(i => new ImageGetDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    URL = i.URL,
                    ProductName = i.Product?.ProductName ?? "Unknown",
                    ProductDescription = i.Product?.Description,
                    Price = i.Product?.Price ?? 0,
                    AvailableProduct = i.Product?.AvailableProduct ?? 0,
                    BrandId = i.Product?.BrandId ?? 0,
                    CategoryId = i.Product?.CategoryId ?? 0,
                    BrandName = i.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = i.Product?.Category?.CategoryName ?? "Unknown"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving images for product ID {productId}", ex);
            }
        }

        public async Task<List<ImageGetDTO>> GetByCategoryIdAsync(int categoryId)
        {
            try
            {
                var images = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .Where(i => i.Product.CategoryId == categoryId)
                    .ToListAsync();

                return images.Select(i => new ImageGetDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    URL = i.URL,
                    ProductName = i.Product?.ProductName ?? "Unknown",
                    ProductDescription = i.Product?.Description,
                    Price = i.Product?.Price ?? 0,
                    AvailableProduct = i.Product?.AvailableProduct ?? 0,
                    BrandId = i.Product?.BrandId ?? 0,
                    CategoryId = i.Product?.CategoryId ?? 0,
                    BrandName = i.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = i.Product?.Category?.CategoryName ?? "Unknown"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving images for category ID {categoryId}", ex);
            }
        }

        public async Task<List<ImageGetDTO>> GetByBrandIdAsync(int brandId)
        {
            try
            {
                var images = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .Where(i => i.Product.BrandId == brandId)
                    .ToListAsync();

                return images.Select(i => new ImageGetDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    URL = i.URL,
                    ProductName = i.Product?.ProductName ?? "Unknown",
                    ProductDescription = i.Product?.Description,
                    Price = i.Product?.Price ?? 0,
                    AvailableProduct = i.Product?.AvailableProduct ?? 0,
                    BrandId = i.Product?.BrandId ?? 0,
                    CategoryId = i.Product?.CategoryId ?? 0,
                    BrandName = i.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = i.Product?.Category?.CategoryName ?? "Unknown"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving images for brand ID {brandId}", ex);
            }
        }

        public async Task<List<ImageGetDTO>> FilterByBrandNameAsync(string brandName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(brandName))
                    return new List<ImageGetDTO>();

                var images = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .Where(i => i.Product.Brand.Name.ToLower().Contains(brandName.ToLower()))
                    .ToListAsync();

                return images.Select(i => new ImageGetDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    URL = i.URL,
                    ProductName = i.Product?.ProductName ?? "Unknown",
                    ProductDescription = i.Product?.Description,
                    Price = i.Product?.Price ?? 0,
                    AvailableProduct = i.Product?.AvailableProduct ?? 0,
                    BrandId = i.Product?.BrandId ?? 0,
                    CategoryId = i.Product?.CategoryId ?? 0,
                    BrandName = i.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = i.Product?.Category?.CategoryName ?? "Unknown"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error filtering images by brand name '{brandName}'", ex);
            }
        }

        public async Task<List<ImageGetDTO>> FilterByCategoryAsync(string categoryName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(categoryName))
                    return new List<ImageGetDTO>();

                var images = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .Where(i => i.Product.Category.CategoryName.ToLower().Contains(categoryName.ToLower()))
                    .ToListAsync();

                return images.Select(i => new ImageGetDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    URL = i.URL,
                    ProductName = i.Product?.ProductName ?? "Unknown",
                    ProductDescription = i.Product?.Description,
                    Price = i.Product?.Price ?? 0,
                    AvailableProduct = i.Product?.AvailableProduct ?? 0,
                    BrandId = i.Product?.BrandId ?? 0,
                    CategoryId = i.Product?.CategoryId ?? 0,
                    BrandName = i.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = i.Product?.Category?.CategoryName ?? "Unknown"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error filtering images by category name '{categoryName}'", ex);
            }
        }

        public async Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file)
        {
            try
            {
                // Find the Product record
                var product = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == dto.ProductId);

                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");
                }

                // Check if an image already exists for this product
                var existingImage = await _imageRepository.GetDbSet()
                    .FirstOrDefaultAsync(i => i.ProductId == dto.ProductId);

                if (existingImage != null)
                {
                    throw new InvalidOperationException($"Product with ID {dto.ProductId} already has an image. Update the existing image instead.");
                }

                var imageUrl = await _imageStorage.UploadImageAsync(file);

                // Create a new image
                var image = new Image
                {
                    URL = imageUrl,
                    ProductId = dto.ProductId
                };

                await _imageRepository.AddAsync(image);
                await _imageRepository.SaveChangesAsync();

                return new ImageGetDTO
                {
                    Id = image.Id,
                    ProductId = image.ProductId,
                    URL = image.URL,
                    ProductName = product.ProductName,
                    ProductDescription = product.Description,
                    Price = product.Price,
                    AvailableProduct = product.AvailableProduct,
                    BrandId = product.BrandId,
                    CategoryId = product.CategoryId,
                    BrandName = product.Brand?.Name ?? "Unknown",
                    CategoryName = product.Category?.CategoryName ?? "Unknown"
                };
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating image", ex);
            }
        }

        public async Task UpdateAsync(ImageUpdateDTO dto, IFormFile newFile = null)
        {
            try
            {
                var image = await _imageRepository.GetByIdAsync(dto.Id);
                if (image == null)
                    throw new KeyNotFoundException($"Image with ID {dto.Id} not found");

                // Validate that the product exists
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                {
                    throw new KeyNotFoundException($"Product with ID {dto.ProductId} not found");
                }

                // Check if another image already exists for this product (excluding current image)
                var existingImage = await _imageRepository.GetDbSet()
                    .FirstOrDefaultAsync(i => i.ProductId == dto.ProductId && i.Id != dto.Id);

                if (existingImage != null)
                {
                    throw new InvalidOperationException($"Product with ID {dto.ProductId} already has another image. Each product can only have one image.");
                }

                // Update image file if provided
                if (newFile != null)
                {
                    var newUrl = await _imageStorage.UploadImageAsync(newFile);
                    await _imageStorage.DeleteImageAsync(image.URL);
                    image.URL = newUrl;
                }

                // Update product reference
                image.ProductId = dto.ProductId;

                await _imageRepository.UpdateAsync(image);
                await _imageRepository.SaveChangesAsync();
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating image with ID {dto.Id}", ex);
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var image = await _imageRepository.GetByIdAsync(id);
                if (image != null)
                {
                    await _imageStorage.DeleteImageAsync(image.URL);
                    await _imageRepository.DeleteAsync(image);
                    await _imageRepository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting image with ID {id}", ex);
            }
        }

        // Backward compatibility methods
        [Obsolete("Use GetByProductIdAsync instead")]
        public async Task<List<ImageGetDTO>> GetByProductIdAsync(int brandId, int categoryId)
        {
            try
            {
                // Find products with the given brand and category
                var productIds = await _context.Products
                    .Where(p => p.BrandId == brandId && p.CategoryId == categoryId)
                    .Select(p => p.Id)
                    .ToListAsync();

                if (!productIds.Any())
                    return new List<ImageGetDTO>();

                var images = await _imageRepository.GetDbSet()
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Category)
                    .Include(i => i.Product)
                        .ThenInclude(p => p.Brand)
                    .Where(i => productIds.Contains(i.ProductId))
                    .ToListAsync();

                return images.Select(i => new ImageGetDTO
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    URL = i.URL,
                    ProductName = i.Product?.ProductName ?? "Unknown",
                    ProductDescription = i.Product?.Description,
                    Price = i.Product?.Price ?? 0,
                    AvailableProduct = i.Product?.AvailableProduct ?? 0,
                    BrandId = i.Product?.BrandId ?? 0,
                    CategoryId = i.Product?.CategoryId ?? 0,
                    BrandName = i.Product?.Brand?.Name ?? "Unknown",
                    CategoryName = i.Product?.Category?.CategoryName ?? "Unknown"
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving images for brand ID {brandId} and category ID {categoryId}", ex);
            }
        }

        // Backward compatibility method for creating images by brand-category
        public async Task<ImageGetDTO> CreateByBrandCategoryAsync(ImageCreateByBrandCategoryDTO dto, IFormFile file)
        {
            try
            {
                // Find products with the given brand and category
                var products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .Where(p => p.BrandId == dto.BrandId && p.CategoryId == dto.CategoryId)
                    .ToListAsync();

                if (!products.Any())
                {
                    throw new KeyNotFoundException($"No products found for brand ID {dto.BrandId} and category ID {dto.CategoryId}");
                }

                if (products.Count > 1)
                {
                    throw new InvalidOperationException($"Multiple products found for brand ID {dto.BrandId} and category ID {dto.CategoryId}. Please specify the exact product ID.");
                }

                var product = products.First();

                // Check if an image already exists for this product
                var existingImage = await _imageRepository.GetDbSet()
                    .FirstOrDefaultAsync(i => i.ProductId == product.Id);

                var imageUrl = await _imageStorage.UploadImageAsync(file);

                if (existingImage != null)
                {
                    // Update the existing image
                    await _imageStorage.DeleteImageAsync(existingImage.URL);
                    existingImage.URL = imageUrl;
                    await _imageRepository.UpdateAsync(existingImage);
                    await _imageRepository.SaveChangesAsync();

                    return new ImageGetDTO
                    {
                        Id = existingImage.Id,
                        ProductId = existingImage.ProductId,
                        URL = existingImage.URL,
                        ProductName = product.ProductName,
                        ProductDescription = product.Description,
                        Price = product.Price,
                        AvailableProduct = product.AvailableProduct,
                        BrandId = product.BrandId,
                        CategoryId = product.CategoryId,
                        BrandName = product.Brand?.Name ?? "Unknown",
                        CategoryName = product.Category?.CategoryName ?? "Unknown"
                    };
                }
                else
                {
                    // Create a new image
                    var image = new Image
                    {
                        URL = imageUrl,
                        ProductId = product.Id
                    };
                    await _imageRepository.AddAsync(image);
                    await _imageRepository.SaveChangesAsync();

                    return new ImageGetDTO
                    {
                        Id = image.Id,
                        ProductId = image.ProductId,
                        URL = image.URL,
                        ProductName = product.ProductName,
                        ProductDescription = product.Description,
                        Price = product.Price,
                        AvailableProduct = product.AvailableProduct,
                        BrandId = product.BrandId,
                        CategoryId = product.CategoryId,
                        BrandName = product.Brand?.Name ?? "Unknown",
                        CategoryName = product.Category?.CategoryName ?? "Unknown"
                    };
                }
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating image for brand ID {dto.BrandId} and category ID {dto.CategoryId}", ex);
            }
        }
    }
}