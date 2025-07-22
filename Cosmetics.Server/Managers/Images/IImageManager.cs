using Cosmetics.Server.Controllers.Images.DTO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cosmetics.Server.Managers.Images
{
    public interface IImageManager
    {
        Task<List<ImageGetDTO>> GetAllAsync();
        Task<ImageGetDTO> GetByIdAsync(int id);
        Task<List<ImageGetDTO>> GetByProductIdAsync(int productId);
        Task<List<ImageGetDTO>> GetByCategoryIdAsync(int categoryId);
        Task<List<ImageGetDTO>> GetByBrandIdAsync(int brandId);
        Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file);
        Task UpdateAsync(ImageUpdateDTO dto, IFormFile newFile = null);
        Task DeleteAsync(int id);
        Task<List<ImageGetDTO>> FilterByBrandNameAsync(string brandName);
        Task<List<ImageGetDTO>> FilterByCategoryAsync(string categoryName);

        // Backward compatibility methods
        [System.Obsolete("Use GetByProductIdAsync instead")]
        Task<List<ImageGetDTO>> GetByProductIdAsync(int brandId, int categoryId);
        Task<ImageGetDTO> CreateByBrandCategoryAsync(ImageCreateByBrandCategoryDTO dto, IFormFile file);
    }
}