using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Models;
using CMS.Server.Controllers.Cloths.DTO;

namespace CMS.Server.Managers.Cloths
{
    public interface IClothManager
    {
        // Keep only 5 basic CRUD methods
        Task<Cloth> CreateClothAsync(Cloth cloth);
        Task<Cloth> UpdateClothAsync(Cloth cloth);
        Task<bool> DeleteClothAsync(int id);
        Task<Cloth> GetClothByIdAsync(int id);
        Task<IEnumerable<Cloth>> GetAllClothsAsync();
    }
}