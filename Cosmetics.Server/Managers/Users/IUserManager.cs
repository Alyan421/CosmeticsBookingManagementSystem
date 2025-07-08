using CMS.Server.Controllers.Users.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.Server.Managers.Users
{
    public interface IUserManager
    {
        Task<UserResponseDTO> RegisterAsync(UserRegisterDTO dto);
        Task<string> LoginAsync(UserLoginDTO dto);
        Task<UserResponseDTO> GetByUsernameAsync(string username);
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO> UpdateUserAsync(int id, UserUpdateDTO dto);
        Task<bool> DeleteUserAsync(int id);
    }
}