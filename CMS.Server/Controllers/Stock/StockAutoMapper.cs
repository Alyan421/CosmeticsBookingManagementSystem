using AutoMapper;
using CMS.Server.Controllers.Stock.DTO;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Stock
{
    public class StockAutoMapper : Profile
    {
        public StockAutoMapper()
        {
            CreateMap<ClothColor, StockDTO>()
                .ForMember(dest => dest.ClothName, opt => opt.MapFrom(src => src.Cloth.Name))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.Color.ColorName));

            CreateMap<StockUpdateDTO, ClothColor>()
                .ForMember(dest => dest.Cloth, opt => opt.Ignore())
                .ForMember(dest => dest.Color, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore());
        }
    }
}