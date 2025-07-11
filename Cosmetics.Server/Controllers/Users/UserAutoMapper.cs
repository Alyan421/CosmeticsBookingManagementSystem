using AutoMapper;
using Cosmetics.Server.Models;
using Cosmetics.Server.Controllers.Users.DTO;

namespace Cosmetics.Server.Controllers.Users
{
    public class UserAutoMapper : Profile
    {
        public UserAutoMapper()
        {
            CreateMap<User, UserResponseDTO>().ReverseMap();
            CreateMap<UserRegisterDTO, User>()
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<UserUpdateDTO, User>()
                    .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
            CreateMap<User, UserLoginDTO>().ReverseMap();
        }
    }
}