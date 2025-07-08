using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Server.Controllers.Brands.DTO;
using CMS.Server.Models;
using CMS.Server.Repository;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CMS.Server.Managers.Brands
{
    public class BrandManager : IBrandManager
    {
        private readonly IGenericRepository<Brand> _brandRepository;
        private readonly IGenericRepository<BrandCategory> _brandCategoryRepository;
        private readonly IMapper _mapper;

        public BrandManager(
            IGenericRepository<Brand> brandRepository,
            IGenericRepository<BrandCategory> brandCategoryRepository,
            IMapper mapper)
        {
            _brandRepository = brandRepository;
            _brandCategoryRepository = brandCategoryRepository;
            _mapper = mapper;
        }

        public async Task<Brand> CreateBrandAsync(Brand brand)
        {
            try
            {
                // Add the brand
                await _brandRepository.AddAsync(brand);
                await _brandRepository.SaveChangesAsync();

                // Return brand with relationships
                return await GetBrandByIdAsync(brand.Id);
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
                // Update the brand
                await _brandRepository.UpdateAsync(brand);
                await _brandRepository.SaveChangesAsync();

                // Return brand with relationships
                return await GetBrandByIdAsync(brand.Id);
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

                var hasAssociatedBrands = await _brandCategoryRepository.ExistsAsync(cc => cc.BrandId == id);
                if (hasAssociatedBrands)
                {
                    throw new InvalidOperationException("Cannot delete brand as it is associated with one or more brands. Please remove these associations first.");
                }

                if (brand == null)
                {
                    return false;
                }

                await _brandRepository.DeleteAsync(brand);
                await _brandRepository.SaveChangesAsync();

                return true;
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
                return await _brandRepository.GetByIdAsync(id);
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
                return await _brandRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all brands", ex);
            }
        }
    }
}