using System.Collections.Generic;
using System.Threading.Tasks;
using Cosmetics.Server.Models;
using Cosmetics.Server.Controllers.Brands.DTO;

namespace Cosmetics.Server.Managers.Brands
{
    public interface IBrandManager
    {
        // Keep only 5 basic CRUD methods
        Task<Brand> CreateBrandAsync(Brand brand);
        Task<Brand> UpdateBrandAsync(Brand brand);
        Task<bool> DeleteBrandAsync(int id);
        Task<Brand> GetBrandByIdAsync(int id);
        Task<IEnumerable<Brand>> GetAllBrandsAsync();
    }
}