using Microsoft.AspNetCore.Mvc;
using Cosmetics.Server.Controllers.Images.DTO;

namespace Cosmetics.Server.Controllers.Images
{
    public interface IImageController
    {
        Task<IActionResult> GetAll();
        Task<IActionResult> GetById(int id);
        Task<IActionResult> GetByProductId(int productId);
        Task<IActionResult> GetByCategory(int categoryId);
        Task<IActionResult> GetByBrand(int brandId);
        Task<IActionResult> Upload([FromForm] ImageCreateDTO dto);
        Task<IActionResult> Update(int id, [FromForm] ImageUpdateDTO dto, [FromForm] IFormFile imageFile = null);
        Task<IActionResult> Delete(int id);
        Task<IActionResult> FilterByBrandName(string brandName);
        Task<IActionResult> FilterByCategory(string categoryName);

        // Backward compatibility methods
        [System.Obsolete("Use GetByProductId instead")]
        Task<IActionResult> GetByBrandCategory(int brandId, int categoryId);

        [System.Obsolete("Use Upload with ProductId instead")]
        Task<IActionResult> UploadByBrandCategory([FromForm] ImageCreateByBrandCategoryDTO dto);

        [System.Obsolete("Use Update with ProductId instead")]
        Task<IActionResult> UpdateByBrandCategory(int id, [FromForm] int brandId, [FromForm] int categoryId, [FromForm] IFormFile imageFile = null);
    }
}