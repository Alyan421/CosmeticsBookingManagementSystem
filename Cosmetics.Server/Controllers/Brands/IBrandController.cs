using Microsoft.AspNetCore.Mvc;
using Cosmetics.Server.Controllers.Brands.DTO;

namespace Cosmetics.Server.Controllers.Brands
{
    public interface IBrandController
    {
        Task<IActionResult> CreateAsync(BrandCreateDTO brandCreateDTO);
        Task<IActionResult> GetByIdAsync(int id);
        Task<IActionResult> UpdateAsync(BrandUpdateDTO brandUpdateDTO);
        Task<IActionResult> DeleteBrandAsync(int id);
        Task<IActionResult> GetAllAsync();

    }
}
