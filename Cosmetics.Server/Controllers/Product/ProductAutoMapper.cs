using AutoMapper;
using CMS.Server.Controllers.Products.DTO;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Products
{
    public class ProductAutoMapper : Profile
    {
        public ProductAutoMapper()
        {
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));

            CreateMap<ProductUpdateDTO, Product>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Image, opt => opt.Ignore());
        }
    }
}