using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Controllers.Categories.DTO;
using CMS.Server.Models;

namespace CMS.Server.Managers.Categories
{
    public interface ICategoryManager
    {
        // 5 basic CRUD methods
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CategoryCreateDTO categoryCreateDTO);
        Task<Category> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDTO);
        Task<bool> DeleteCategoryAsync(int id);
    }
}