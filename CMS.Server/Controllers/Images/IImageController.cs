using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Images.DTO;

namespace CMS.Server.Controllers.Images
{
    public interface IImageController
    {
        Task<IActionResult> GetAll();
        Task<IActionResult> GetByColor(int colorId);
        Task<IActionResult> GetById(int id);
        Task<IActionResult> Upload(ImageCreateDTO dto);
        Task<IActionResult> Update(int id,[FromForm] int clothId,[FromForm] int colorId,[FromForm] IFormFile imagefile = null);

        Task<IActionResult> Delete(int id);
        Task<IActionResult> FilterByClothName(string clothName);
        Task<IActionResult> FilterByColor(string colorName);
    }
}
