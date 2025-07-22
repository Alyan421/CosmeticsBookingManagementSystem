using AutoMapper;
using Cosmetics.Server.Controllers.Categories.DTO;
using Cosmetics.Server.Models;
using Cosmetics.Server.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cosmetics.Server.Managers.Categories
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public CategoryManager(
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Product> productRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                return await _categoryRepository.GetDbSet()
                    .Include(c => c.Products)
                        .ThenInclude(p => p.Brand)
                    .Include(c => c.Products)
                        .ThenInclude(p => p.Image)
                    .ToListAsync();
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
                var category = await _categoryRepository.GetDbSet()
                    .Include(c => c.Products)
                        .ThenInclude(p => p.Brand)
                    .Include(c => c.Products)
                        .ThenInclude(p => p.Image)
                    .FirstOrDefaultAsync(c => c.Id == id);

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
                // Validate that category name is unique
                var existingCategory = await _categoryRepository.GetDbSet()
                    .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryCreateDTO.CategoryName.ToLower());

                if (existingCategory != null)
                {
                    throw new InvalidOperationException($"Category with name '{categoryCreateDTO.CategoryName}' already exists.");
                }

                // Map DTO to entity
                var category = _mapper.Map<Category>(categoryCreateDTO);

                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();

                // Return the created category with relationships
                return await GetCategoryByIdAsync(category.Id);
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule violations as-is
                throw;
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

                // Validate that category name is unique (excluding current category)
                var duplicateCategory = await _categoryRepository.GetDbSet()
                    .FirstOrDefaultAsync(c => c.Id != categoryUpdateDTO.Id &&
                                           c.CategoryName.ToLower() == categoryUpdateDTO.CategoryName.ToLower());

                if (duplicateCategory != null)
                {
                    throw new InvalidOperationException($"Category with name '{categoryUpdateDTO.CategoryName}' already exists.");
                }

                // Update properties
                _mapper.Map(categoryUpdateDTO, category);
                await _categoryRepository.UpdateAsync(category);
                await _categoryRepository.SaveChangesAsync();

                // Return the updated category with relationships
                return await GetCategoryByIdAsync(category.Id);
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

                // Check if category is associated with any products
                var hasAssociatedProducts = await _productRepository.ExistsAsync(p => p.CategoryId == id);
                if (hasAssociatedProducts)
                {
                    throw new InvalidOperationException("Cannot delete category as it is associated with one or more products. Please remove these associations first.");
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

        // Additional helper methods for product management
        public async Task<IEnumerable<Product>> GetCategoryProductsAsync(int categoryId)
        {
            try
            {
                var category = await GetCategoryByIdAsync(categoryId);
                return category.Products;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving products for category with ID {categoryId}", ex);
            }
        }

        public async Task<int> GetCategoryProductCountAsync(int categoryId)
        {
            try
            {
                return await _productRepository.GetDbSet()
                    .CountAsync(p => p.CategoryId == categoryId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting products for category with ID {categoryId}", ex);
            }
        }

        public async Task<IEnumerable<Category>> SearchCategoriesByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllCategoriesAsync();
                }

                return await _categoryRepository.GetDbSet()
                    .Include(c => c.Products)
                        .ThenInclude(p => p.Brand)
                    .Include(c => c.Products)
                        .ThenInclude(p => p.Image)
                    .Where(c => c.CategoryName.ToLower().Contains(searchTerm.ToLower()))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching categories with term '{searchTerm}'", ex);
            }
        }
    }
}