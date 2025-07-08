using AutoMapper;
using CMS.Server.Controllers.Users.DTO;
using CMS.Server.Models;
using CMS.Server.Repository;
using CMS.Server.Services;
using System.Text.RegularExpressions;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMS.Server.Managers.Users
{
    public class UserManager : IUserManager
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserManager(IGenericRepository<User> userRepository, IConfiguration config, IMapper mapper)
        {
            _userRepository = userRepository;
            _config = config;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> RegisterAsync(UserRegisterDTO dto)
        {
            // Validate email format
            if (!IsValidEmail(dto.Email))
                throw new ArgumentException("Invalid email format.");

            // Check for duplicate username
            if (await _userRepository.ExistsAsync(u => u.Username == dto.Username))
                throw new ArgumentException("Username already exists.");

            // Map DTO to entity
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            await _userRepository.AddAsync(user);
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<UserResponseDTO> UpdateUserAsync(int id, UserUpdateDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return null;

            // Validate email format
            if (!IsValidEmail(dto.Email))
                throw new ArgumentException("Invalid email format.");

            // Check for duplicate username (excluding the current user)
            if (await _userRepository.ExistsAsync(u => u.Username == dto.Username && u.Id != id))
                throw new ArgumentException("Username already exists.");

            user.Username = dto.Username;
            user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                // Hash only if password is provided
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<string> LoginAsync(UserLoginDTO dto)
        {
            var user = (await _userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Username == dto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return JwtTokenGenerator.GenerateToken(user, _config);
        }

        public async Task<UserResponseDTO> GetByUsernameAsync(string username)
        {
            var user = await _userRepository.GetSingleAsync(u => u.Username == username);
            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserResponseDTO>>(users);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            await _userRepository.DeleteAsync(user);
            return true;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Regular expression for validating email format
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailRegex);
        }
    }

}
