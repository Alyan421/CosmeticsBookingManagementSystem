using AutoMapper;
using CMS.Server.Controllers.Categories.DTO;
using CMS.Server.Models;
using CMS.Server.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CMS.Server.Managers.Categories
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<BrandCategory> _brandCategoryRepository;
        private readonly IMapper _mapper;

        public CategoryManager(
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<BrandCategory> brandCategoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _brandCategoryRepository = brandCategoryRepository;
            _mapper = mapper;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                var query = _categoryRepository.GetDbSet()
                    .Include(c => c.BrandCategories)
                    .ThenInclude(cc => cc.Brand);

                return (await _categoryRepository.GetListAsync(c => true)).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error retrieving categories", ex);
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);

                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }

                return category;
            }
            catch (KeyNotFoundException)
            {
                // Rethrow key not found exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error retrieving category with ID {id}", ex);
            }
        }

        public async Task<Category> CreateCategoryAsync(CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                // Map DTO to entity
                var category = _mapper.Map<Category>(categoryCreateDTO);

                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();

                // Return the created category
                return await GetCategoryByIdAsync(category.Id);
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error creating category", ex);
            }
        }

        public async Task<Category> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDTO)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(categoryUpdateDTO.Id);
                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {categoryUpdateDTO.Id} not found");
                }

                // Update properties
                _mapper.Map(categoryUpdateDTO, category);
                await _categoryRepository.UpdateAsync(category);
                await _categoryRepository.SaveChangesAsync();

                // Return the updated category
                return await GetCategoryByIdAsync(category.Id);
            }
            catch (KeyNotFoundException)
            {
                // Rethrow key not found exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error updating category with ID {categoryUpdateDTO.Id}", ex);
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return false; // Category not found
                }

                // Check if category is associated with any brands
                var hasAssociatedBrands = await _brandCategoryRepository.ExistsAsync(cc => cc.CategoryId == id);
                if (hasAssociatedBrands)
                {
                    throw new InvalidOperationException("Cannot delete category as it is associated with one or more brands. Please remove these associations first.");
                }

                await _categoryRepository.DeleteAsync(category);
                await _categoryRepository.SaveChangesAsync();

                return true; // Successfully deleted
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule violations as-is
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception($"Error deleting category with ID {id}", ex);
            }
        }
    }
}