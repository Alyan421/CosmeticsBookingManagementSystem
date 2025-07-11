using Microsoft.AspNetCore.Mvc;
using Cosmetics.Server.Controllers.Images.DTO;

namespace Cosmetics.Server.Controllers.Images
{
    public interface IImageController
    {
        Task<IActionResult> GetAll();
        Task<IActionResult> GetByCategory(int categoryId);
        Task<IActionResult> GetById(int id);
        Task<IActionResult> Upload(ImageCreateDTO dto);
        Task<IActionResult> Update(int id,[FromForm] int brandId,[FromForm] int categoryId,[FromForm] IFormFile imagefile = null);

        Task<IActionResult> Delete(int id);
        Task<IActionResult> FilterByBrandName(string brandName);
        Task<IActionResult> FilterByCategory(string categoryName);
    }
}
