using System.Collections.Generic;
using System.Threading.Tasks;
using CMS.Server.Controllers.Colors.DTO;
using CMS.Server.Models;

namespace CMS.Server.Managers.Colors
{
    public interface IColorManager
    {
        // 5 basic CRUD methods
        Task<List<Color>> GetAllColorsAsync();
        Task<Color> GetColorByIdAsync(int id);
        Task<Color> CreateColorAsync(ColorCreateDTO colorCreateDTO);
        Task<Color> UpdateColorAsync(ColorUpdateDTO colorUpdateDTO);
        Task<bool> DeleteColorAsync(int id);
    }
}