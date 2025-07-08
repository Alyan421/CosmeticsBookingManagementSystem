using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Models;
using CMS.Server.Controllers.Brands.DTO;

namespace CMS.Server.Managers.Brands
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