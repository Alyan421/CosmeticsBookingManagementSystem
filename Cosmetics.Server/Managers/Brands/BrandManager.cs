using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosmetics.Server.Controllers.Brands.DTO;
using Cosmetics.Server.Models;
using Cosmetics.Server.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Cosmetics.Server.Managers.Brands
{
    public class BrandManager : IBrandManager
    {
        private readonly IGenericRepository<Brand> _brandRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public BrandManager(
            IGenericRepository<Brand> brandRepository,
            IGenericRepository<Product> productRepository,
            IMapper mapper)
        {
            _brandRepository = brandRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Brand> CreateBrandAsync(Brand brand)
        {
            try
            {
                // Validate that brand name is unique
                var existingBrand = await _brandRepository.GetDbSet()
                    .FirstOrDefaultAsync(b => b.Name.ToLower() == brand.Name.ToLower());

                if (existingBrand != null)
                {
                    throw new InvalidOperationException($"Brand with name '{brand.Name}' already exists.");
                }

                // Add the brand
                await _brandRepository.AddAsync(brand);
                await _brandRepository.SaveChangesAsync();

                // Return brand with relationships
                return await GetBrandByIdAsync(brand.Id);
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule violations as-is
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating brand", ex);
            }
        }

        public async Task<Brand> UpdateBrandAsync(Brand brand)
        {
            try
            {
                // Check if brand exists
                var existingBrand = await _brandRepository.GetByIdAsync(brand.Id);
                if (existingBrand == null)
                {
                    throw new KeyNotFoundException($"Brand with ID {brand.Id} not found");
                }

                // Validate that brand name is unique (excluding current brand)
                var duplicateBrand = await _brandRepository.GetDbSet()
                    .FirstOrDefaultAsync(b => b.Id != brand.Id && b.Name.ToLower() == brand.Name.ToLower());

                if (duplicateBrand != null)
                {
                    throw new InvalidOperationException($"Brand with name '{brand.Name}' already exists.");
                }

                // Update the brand
                await _brandRepository.UpdateAsync(brand);
                await _brandRepository.SaveChangesAsync();

                // Return brand with relationships
                return await GetBrandByIdAsync(brand.Id);
            }
            catch (KeyNotFoundException)
            {
                // Rethrow key not found exceptions as-is
                throw;
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule violations as-is
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating brand with ID {brand.Id}", ex);
            }
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            try
            {
                var brand = await _brandRepository.GetByIdAsync(id);
                if (brand == null)
                {
                    return false; // Brand not found
                }

                // Check if brand is associated with any products
                var hasAssociatedProducts = await _productRepository.ExistsAsync(p => p.BrandId == id);
                if (hasAssociatedProducts)
                {
                    throw new InvalidOperationException("Cannot delete brand as it is associated with one or more products. Please remove these associations first.");
                }

                await _brandRepository.DeleteAsync(brand);
                await _brandRepository.SaveChangesAsync();

                return true;
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule violations as-is
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting brand with ID {id}", ex);
            }
        }

        public async Task<Brand> GetBrandByIdAsync(int id)
        {
            try
            {
                var brand = await _brandRepository.GetDbSet()
                    .Include(b => b.Products)
                        .ThenInclude(p => p.Category)
                    .Include(b => b.Products)
                        .ThenInclude(p => p.Image)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (brand == null)
                {
                    throw new KeyNotFoundException($"Brand with ID {id} not found");
                }

                return brand;
            }
            catch (KeyNotFoundException)
            {
                // Rethrow key not found exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving brand with ID {id}", ex);
            }
        }

        public async Task<IEnumerable<Brand>> GetAllBrandsAsync()
        {
            try
            {
                return await _brandRepository.GetDbSet()
                    .Include(b => b.Products)
                        .ThenInclude(p => p.Category)
                    .Include(b => b.Products)
                        .ThenInclude(p => p.Image)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all brands", ex);
            }
        }

        // Additional helper methods for product management
        public async Task<IEnumerable<Product>> GetBrandProductsAsync(int brandId)
        {
            try
            {
                var brand = await GetBrandByIdAsync(brandId);
                return brand.Products;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving products for brand with ID {brandId}", ex);
            }
        }

        public async Task<int> GetBrandProductCountAsync(int brandId)
        {
            try
            {
                return await _productRepository.GetDbSet()
                    .CountAsync(p => p.BrandId == brandId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting products for brand with ID {brandId}", ex);
            }
        }
    }
}