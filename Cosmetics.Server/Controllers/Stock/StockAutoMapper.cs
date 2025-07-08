using AutoMapper;
using CMS.Server.Controllers.Stock.DTO;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Stock
{
    public class StockAutoMapper : Profile
    {
        public StockAutoMapper()
        {
            CreateMap<BrandCategory, StockDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<StockUpdateDTO, BrandCategory>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore());
        }
    }
}