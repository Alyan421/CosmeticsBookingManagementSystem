using CMS.Server.Controllers.Images.DTO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Server.Managers.Images
{
    public interface IImageManager
    {
        Task<List<ImageGetDTO>> GetAllAsync();
        Task<List<ImageGetDTO>> GetByCategoryIdAsync(int categoryId);
        Task<ImageGetDTO> GetByIdAsync(int id);
        Task<ImageGetDTO> CreateAsync(ImageCreateDTO dto, IFormFile file);
        Task UpdateAsync(ImageUpdateDTO dto, IFormFile newFile = null);
        Task DeleteAsync(int id);
        Task<List<ImageGetDTO>> FilterByBrandNameAsync(string brandName);
        Task<List<ImageGetDTO>> FilterByCategoryAsync(string categoryName);
        Task<List<ImageGetDTO>> GetByBrandCategoryIdAsync(int brandId, int categoryId);
    }
}