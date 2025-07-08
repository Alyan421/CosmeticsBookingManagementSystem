using Microsoft.AspNetCore.Mvc;
using CMS.Server.Controllers.Brands.DTO;

namespace CMS.Server.Controllers.Brands
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
