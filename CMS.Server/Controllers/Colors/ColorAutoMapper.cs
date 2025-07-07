using AutoMapper;
using CMS.Server.Models;
using CMS.Server.Controllers.Colors.DTO;
using System.Linq;

namespace CMS.Server.Controllers.Colors
{
    public class ColorAutoMapper : Profile
    {
        public ColorAutoMapper()
        {
            // Map from entity to DTO
            CreateMap<Color, ColorGetDTO>()
                .ForMember(dest => dest.ClothColors, opt => opt.MapFrom(src =>
                    src.ClothColors.Select(cc => new ClothColorInfoDTO
                    {
                        ClothId = cc.ClothId,
                        ClothName = cc.Cloth.Name,
                        AvailableStock = cc.AvailableStock,
                    }).ToList()));

            // Map from DTO to entity
            CreateMap<ColorCreateDTO, Color>();
            CreateMap<ColorUpdateDTO, Color>();
        }
    }
}