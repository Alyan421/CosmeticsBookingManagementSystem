using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Cloths.DTO;

namespace CMS.Server.Controllers.Cloths
{
    public interface IClothController
    {
        Task<IActionResult> CreateAsync(ClothCreateDTO clothCreateDTO);
        Task<IActionResult> GetByIdAsync(int id);
        Task<IActionResult> UpdateAsync(ClothUpdateDTO clothUpdateDTO);
        Task<IActionResult> DeleteClothAsync(int id);
        Task<IActionResult> GetAllAsync();

    }
}
