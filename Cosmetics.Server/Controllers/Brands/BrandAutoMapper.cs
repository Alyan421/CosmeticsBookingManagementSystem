using AutoMapper;
using Cosmetics.Server.Models;
using Cosmetics.Server.Controllers.Brands.DTO;
using System.Linq;

namespace Cosmetics.Server.Controllers.Brands
{
    public class BrandAutoMapper : Profile
    {
        public BrandAutoMapper()
        {
            // Map from entity to DTO
            CreateMap<Brand, BrandGetDTO>()
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.Products.Select(cc => new CategoryInfoDTO
                    {
                        Id = cc.Category.Id,
                        CategoryName = cc.Category.CategoryName,
                    }).ToList()));

            // Map from DTO to entity
            CreateMap<BrandCreateDTO, Brand>();

            CreateMap<BrandUpdateDTO, Brand>();


        }
    }
}