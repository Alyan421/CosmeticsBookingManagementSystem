using CMS.Server.Models;
using CMS.Server.Repository;
using CMS.Server.Controllers.Images.DTO;
using CMS.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using CMS.Server.EntityFrameworkCore;

namespace CMS.Server.Managers.Images
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
            // Get all images with their brand-category relationships
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Category)
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Brand)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                BrandId = i.BrandId,
                CategoryId = i.CategoryId,
                URL = i.URL,
                CategoryName = i.Product?.Category?.CategoryName ?? "Unknown",
                BrandName = i.Product?.Brand?.Name ?? "Unknown",
                Price = i.Product?.Price ?? 0, // Updated: Get Price from Product
                AvailableProduct = i.Product?.AvailableProduct ?? 0
            }).ToList();
        }

        public async Task<List<ImageGetDTO>> GetByProductIdAsync(int brandId, int categoryId)
        {
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Category)
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Brand)
                .Where(i => i.BrandId == brandId && i.CategoryId == categoryId)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                BrandId = i.BrandId,
                CategoryId = i.CategoryId,
                URL = i.URL,
                CategoryName = i.Product?.Category?.CategoryName ?? "Unknown",
                BrandName = i.Product?.Brand?.Name ?? "Unknown",
                Price = i.Product?.Price ?? 0, // Updated: Get Price from Product
                AvailableProduct = i.Product?.AvailableProduct ?? 0
            }).ToList();
        }

        // Keep this method for backward compatibility
        public async Task<List<ImageGetDTO>> GetByCategoryIdAsync(int categoryId)
        {
            // Find all brand-category combinations with this category
            var products = await _context.Products
                .Where(cc => cc.CategoryId == categoryId)
                .ToListAsync();

            if (!products.Any())
                return new List<ImageGetDTO>();

            // Get images for these brand-category combinations
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Category)
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Brand)
                .Where(i => i.CategoryId == categoryId)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                BrandId = i.BrandId,
                CategoryId = i.CategoryId,
                URL = i.URL,
                CategoryName = i.Product?.Category?.CategoryName ?? "Unknown",
                BrandName = i.Product?.Brand?.Name ?? "Unknown",
                Price = i.Product?.Price ?? 0, // Updated: Get Price from Product
                AvailableProduct = i.Product?.AvailableProduct ?? 0
            }).ToList();
        }

        public async Task<ImageGetDTO> GetByIdAsync(int id)
        {
            var image = await _imageRepository.GetDbSet()
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Category)
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Brand)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null) return null;

            return new ImageGetDTO
            {
                Id = image.Id,
                BrandId = image.BrandId,
                CategoryId = image.CategoryId,
                URL = image.URL,
                CategoryName = image.Product?.Category?.CategoryName ?? "Unknown",
                BrandName = image.Product?.Brand?.Name ?? "Unknown",
                Price = image.Product?.Price ?? 0, // Updated: Get Price from Product
                AvailableProduct = image.Product?.AvailableProduct ?? 0
            };
        }

        public async Task<List<ImageGetDTO>> FilterByBrandNameAsync(string brandName)
        {
            // Find brand IDs that match the name
            var brandIds = await _context.Brands
                .Where(c => c.Name.Contains(brandName))
                .Select(c => c.Id)
                .ToListAsync();

            if (!brandIds.Any())
                return new List<ImageGetDTO>();

            // Get images for these brands
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Category)
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Brand)
                .Where(i => brandIds.Contains(i.BrandId))
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                BrandId = i.BrandId,
                CategoryId = i.CategoryId,
                URL = i.URL,
                CategoryName = i.Product?.Category?.CategoryName ?? "Unknown",
                BrandName = i.Product?.Brand?.Name ?? "Unknown",
                Price = i.Product?.Price ?? 0, // Updated: Get Price from Product
                AvailableProduct = i.Product?.AvailableProduct ?? 0
            }).ToList();
        }

        public async Task<List<ImageGetDTO>> FilterByCategoryAsync(string categoryName)
        {
            // Find category IDs that match the name
            var categoryIds = await _context.Categories
                .Where(c => c.CategoryName.Contains(categoryName))
                .Select(c => c.Id)
                .ToListAsync();

            if (!categoryIds.Any())
                return new List<ImageGetDTO>();

            // Get images for these categories
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Category)
                .Include(i => i.Product)
                .ThenInclude(cc => cc.Brand)
                .Where(i => categoryIds.Contains(i.CategoryId))
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                BrandId = i.BrandId,
                CategoryId = i.CategoryId,
                URL = i.URL,
                CategoryName = i.Product?.Category?.CategoryName ?? "Unknown",
                BrandName = i.Product?.Brand?.Name ?? "Unknown",
                Price = i.Product?.Price ?? 0, // Updated: Get Price from Product
                AvailableProduct = i.Product?.AvailableProduct ?? 0
            }).ToList();
        }

        public async Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file)
        {
            // Find the Product record
            var product = await _context.Products
                .Include(cc => cc.Brand)
                .Include(cc => cc.Category)
                .FirstOrDefaultAsync(cc => cc.BrandId == dto.BrandId && cc.CategoryId == dto.CategoryId);

            if (product == null)
            {
                throw new KeyNotFoundException($"No relationship found between brand ID {dto.BrandId} and category ID {dto.CategoryId}");
            }

            // Check if an image already exists for this brand-category combination
            var existingImage = await _imageRepository.GetDbSet()
                .FirstOrDefaultAsync(i => i.BrandId == dto.BrandId && i.CategoryId == dto.CategoryId);

            var imageUrl = await _imageStorage.UploadImageAsync(file);

            if (existingImage != null)
            {
                // Optionally delete the old image file
                await _imageStorage.DeleteImageAsync(existingImage.URL);

                // Update the existing image
                existingImage.URL = imageUrl;
                await _imageRepository.UpdateAsync(existingImage);
                await _imageRepository.SaveChangesAsync();

                return new ImageGetDTO
                {
                    Id = existingImage.Id,
                    BrandId = existingImage.BrandId,
                    CategoryId = existingImage.CategoryId,
                    URL = existingImage.URL,
                    CategoryName = product.Category?.CategoryName ?? "Unknown",
                    BrandName = product.Brand?.Name ?? "Unknown",
                    Price = product.Price, // Updated: Get Price from Product
                    AvailableProduct = product.AvailableProduct
                };
            }
            else
            {
                // Create a new image
                var image = new Image
                {
                    URL = imageUrl,
                    BrandId = dto.BrandId,
                    CategoryId = dto.CategoryId
                };
                await _imageRepository.AddAsync(image);
                await _imageRepository.SaveChangesAsync();

                return new ImageGetDTO
                {
                    Id = image.Id,
                    BrandId = image.BrandId,
                    CategoryId = image.CategoryId,
                    URL = image.URL,
                    CategoryName = product.Category?.CategoryName ?? "Unknown",
                    BrandName = product.Brand?.Name ?? "Unknown",
                    Price = product.Price, // Updated: Get Price from Product
                    AvailableProduct = product.AvailableProduct
                };
            }
        }

        public async Task UpdateAsync(ImageUpdateDTO dto, IFormFile newFile = null)
        {
            var image = await _imageRepository.GetByIdAsync(dto.Id);
            if (image == null) throw new Exception("Image not found");

            if (newFile != null)
            {
                var newUrl = await _imageStorage.UploadImageAsync(newFile);
                await _imageStorage.DeleteImageAsync(image.URL);
                image.URL = newUrl;
            }

            // Find the Product record
            var product = await _context.Products
                .FirstOrDefaultAsync(cc => cc.BrandId == dto.BrandId && cc.CategoryId == dto.CategoryId);

            if (product == null)
            {
                throw new KeyNotFoundException($"No relationship found between brand ID {dto.BrandId} and category ID {dto.CategoryId}");
            }

            image.BrandId = dto.BrandId;
            image.CategoryId = dto.CategoryId;

            await _imageRepository.UpdateAsync(image);
            await _imageRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var image = await _imageRepository.GetByIdAsync(id);
            if (image != null)
            {
                await _imageStorage.DeleteImageAsync(image.URL);
                await _imageRepository.DeleteAsync(image);
                await _imageRepository.SaveChangesAsync();
            }
        }
    }
}