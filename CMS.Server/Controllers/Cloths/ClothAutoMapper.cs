using AutoMapper;
using CMS.Server.Models;
using CMS.Server.Controllers.Cloths.DTO;
using System.Linq;

namespace CMS.Server.Controllers.Cloths
{
    public class ClothAutoMapper : Profile
    {
        public ClothAutoMapper()
        {
            // Map from entity to DTO
            CreateMap<Cloth, ClothGetDTO>()
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src =>
                    src.ClothColors.Select(cc => new ColorInfoDTO
                    {
                        Id = cc.Color.Id,
                        ColorName = cc.Color.ColorName,
                    }).ToList()));

            // Map from DTO to entity
            CreateMap<ClothCreateDTO, Cloth>();

            CreateMap<ClothUpdateDTO, Cloth>();


        }
    }
}