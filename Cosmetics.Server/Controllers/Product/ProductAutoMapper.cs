using AutoMapper;
using Cosmetics.Server.Controllers.Products.DTO;
using Cosmetics.Server.Models;

namespace Cosmetics.Server.Controllers.Products
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