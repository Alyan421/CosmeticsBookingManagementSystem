using AutoMapper;
using CMS.Server.Models;
using CMS.Server.Controllers.Categories.DTO;
using System.Linq;

namespace CMS.Server.Controllers.Categories
{
    public class CategoryAutoMapper : Profile
    {
        public CategoryAutoMapper()
        {
            // Map from entity to DTO
            CreateMap<Category, CategoryGetDTO>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                    src.Products.Select(cc => new ProductInfoDTO
                    {
                        BrandId = cc.BrandId,
                        BrandName = cc.Brand.Name,
                        AvailableProduct = cc.AvailableProduct,
                    }).ToList()));

            // Map from DTO to entity
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
        }
    }
}