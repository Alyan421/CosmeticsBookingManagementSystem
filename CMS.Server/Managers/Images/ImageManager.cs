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
        private readonly IGenericRepository<ClothColor> _clothColorRepository;
        private readonly IImageStorageService _imageStorage;
        private readonly AMSDbContext _context;

        public ImageManager(
            IGenericRepository<Image> imageRepository,
            IGenericRepository<ClothColor> clothColorRepository,
            IImageStorageService imageStorage,
            AMSDbContext context)
        {
            _imageRepository = imageRepository;
            _clothColorRepository = clothColorRepository;
            _imageStorage = imageStorage;
            _context = context;
        }

        public async Task<List<ImageGetDTO>> GetAllAsync()
        {
            // Get all images with their cloth-color relationships
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Color)
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Cloth)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ClothId = i.ClothId,
                ColorId = i.ColorId,
                URL = i.URL,
                ColorName = i.ClothColor?.Color?.ColorName ?? "Unknown",
                ClothName = i.ClothColor?.Cloth?.Name ?? "Unknown",
                Price = i.ClothColor?.Cloth?.Price ?? 0,
                AvailableStock = i.ClothColor?.AvailableStock ?? 0
            }).ToList();
        }

        public async Task<List<ImageGetDTO>> GetByClothColorIdAsync(int clothId, int colorId)
        {
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Color)
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Cloth)
                .Where(i => i.ClothId == clothId && i.ColorId == colorId)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ClothId = i.ClothId,
                ColorId = i.ColorId,
                URL = i.URL,
                ColorName = i.ClothColor?.Color?.ColorName ?? "Unknown",
                ClothName = i.ClothColor?.Cloth?.Name ?? "Unknown",
                Price = i.ClothColor?.Cloth?.Price ?? 0,
                AvailableStock = i.ClothColor?.AvailableStock ?? 0
            }).ToList();
        }

        // Keep this method for backward compatibility
        public async Task<List<ImageGetDTO>> GetByColorIdAsync(int colorId)
        {
            // Find all cloth-color combinations with this color
            var clothColors = await _context.ClothColors
                .Where(cc => cc.ColorId == colorId)
                .ToListAsync();

            if (!clothColors.Any())
                return new List<ImageGetDTO>();

            // Get images for these cloth-color combinations
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Color)
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Cloth)
                .Where(i => i.ColorId == colorId)
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ClothId = i.ClothId,
                ColorId = i.ColorId,
                URL = i.URL,
                ColorName = i.ClothColor?.Color?.ColorName ?? "Unknown",
                ClothName = i.ClothColor?.Cloth?.Name ?? "Unknown",
                Price = i.ClothColor?.Cloth?.Price ?? 0,
                AvailableStock = i.ClothColor?.AvailableStock ?? 0
            }).ToList();
        }

        public async Task<ImageGetDTO> GetByIdAsync(int id)
        {
            var image = await _imageRepository.GetDbSet()
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Color)
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Cloth)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null) return null;

            return new ImageGetDTO
            {
                Id = image.Id,
                ClothId = image.ClothId,
                ColorId = image.ColorId,
                URL = image.URL,
                ColorName = image.ClothColor?.Color?.ColorName ?? "Unknown",
                ClothName = image.ClothColor?.Cloth?.Name ?? "Unknown",
                Price = image.ClothColor?.Cloth?.Price ?? 0,
                AvailableStock = image.ClothColor?.AvailableStock ?? 0
            };
        }

        public async Task<List<ImageGetDTO>> FilterByClothNameAsync(string clothName)
        {
            // Find cloth IDs that match the name
            var clothIds = await _context.Cloths
                .Where(c => c.Name.Contains(clothName))
                .Select(c => c.Id)
                .ToListAsync();

            if (!clothIds.Any())
                return new List<ImageGetDTO>();

            // Get images for these cloths
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Color)
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Cloth)
                .Where(i => clothIds.Contains(i.ClothId))
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ClothId = i.ClothId,
                ColorId = i.ColorId,
                URL = i.URL,
                ColorName = i.ClothColor?.Color?.ColorName ?? "Unknown",
                ClothName = i.ClothColor?.Cloth?.Name ?? "Unknown",
                Price = i.ClothColor?.Cloth?.Price ?? 0,
                AvailableStock = i.ClothColor?.AvailableStock ?? 0
            }).ToList();
        }

        public async Task<List<ImageGetDTO>> FilterByColorAsync(string colorName)
        {
            // Find color IDs that match the name
            var colorIds = await _context.Colors
                .Where(c => c.ColorName.Contains(colorName))
                .Select(c => c.Id)
                .ToListAsync();

            if (!colorIds.Any())
                return new List<ImageGetDTO>();

            // Get images for these colors
            var images = await _imageRepository.GetDbSet()
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Color)
                .Include(i => i.ClothColor)
                .ThenInclude(cc => cc.Cloth)
                .Where(i => colorIds.Contains(i.ColorId))
                .ToListAsync();

            return images.Select(i => new ImageGetDTO
            {
                Id = i.Id,
                ClothId = i.ClothId,
                ColorId = i.ColorId,
                URL = i.URL,
                ColorName = i.ClothColor?.Color?.ColorName ?? "Unknown",
                ClothName = i.ClothColor?.Cloth?.Name ?? "Unknown",
                Price = i.ClothColor?.Cloth?.Price ?? 0,
                AvailableStock = i.ClothColor?.AvailableStock ?? 0
            }).ToList();
        }

        public async Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file)
        {
            // Find the ClothColor record
            var clothColor = await _context.ClothColors
                .Include(cc => cc.Cloth)
                .Include(cc => cc.Color)
                .FirstOrDefaultAsync(cc => cc.ClothId == dto.ClothId && cc.ColorId == dto.ColorId);

            if (clothColor == null)
            {
                throw new KeyNotFoundException($"No relationship found between cloth ID {dto.ClothId} and color ID {dto.ColorId}");
            }

            // Check if an image already exists for this cloth-color combination
            var existingImage = await _imageRepository.GetDbSet()
                .FirstOrDefaultAsync(i => i.ClothId == dto.ClothId && i.ColorId == dto.ColorId);

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
                    ClothId = existingImage.ClothId,
                    ColorId = existingImage.ColorId,
                    URL = existingImage.URL,
                    ColorName = clothColor.Color?.ColorName ?? "Unknown",
                    ClothName = clothColor.Cloth?.Name ?? "Unknown",
                    Price = clothColor.Cloth?.Price ?? 0,
                    AvailableStock = clothColor.AvailableStock
                };
            }
            else
            {
                // Create a new image
                var image = new Image
                {
                    URL = imageUrl,
                    ClothId = dto.ClothId,
                    ColorId = dto.ColorId
                };
                await _imageRepository.AddAsync(image);
                await _imageRepository.SaveChangesAsync();

                return new ImageGetDTO
                {
                    Id = image.Id,
                    ClothId = image.ClothId,
                    ColorId = image.ColorId,
                    URL = image.URL,
                    ColorName = clothColor.Color?.ColorName ?? "Unknown",
                    ClothName = clothColor.Cloth?.Name ?? "Unknown",
                    Price = clothColor.Cloth?.Price ?? 0,
                    AvailableStock = clothColor.AvailableStock
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

            // Find the ClothColor record
            var clothColor = await _context.ClothColors
                .FirstOrDefaultAsync(cc => cc.ClothId == dto.ClothId && cc.ColorId == dto.ColorId);

            if (clothColor == null)
            {
                throw new KeyNotFoundException($"No relationship found between cloth ID {dto.ClothId} and color ID {dto.ColorId}");
            }

            image.ClothId = dto.ClothId;
            image.ColorId = dto.ColorId;

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