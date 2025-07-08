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
                .ForMember(dest => dest.BrandCategories, opt => opt.MapFrom(src =>
                    src.BrandCategories.Select(cc => new BrandCategoryInfoDTO
                    {
                        BrandId = cc.BrandId,
                        BrandName = cc.Brand.Name,
                        AvailableStock = cc.AvailableStock,
                    }).ToList()));

            // Map from DTO to entity
            CreateMap<CategoryCreateDTO, Category>();
            CreateMap<CategoryUpdateDTO, Category>();
        }
    }
}