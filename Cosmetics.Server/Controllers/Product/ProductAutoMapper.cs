using AutoMapper;
using Cosmetics.Server.Controllers.Products.DTO;
using Cosmetics.Server.Models;

namespace Cosmetics.Server.Controllers.Products
{
    public class ProductAutoMapper : Profile
    {
        public ProductAutoMapper()
        {
            // Map Product entity to ProductDTO
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image != null ? src.Image.URL : null));

            // Map ProductCreateDTO to Product entity
            CreateMap<ProductCreateDTO, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id is auto-generated
                .ForMember(dest => dest.Brand, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.Image, opt => opt.Ignore()); // Navigation property

            // Map ProductUpdateDTO to Product entity
            CreateMap<ProductUpdateDTO, Product>()
                .ForMember(dest => dest.Brand, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.Category, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.Image, opt => opt.Ignore()); // Navigation property

            // Map Product entity to ProductSummaryDTO
            CreateMap<Product, ProductSummaryDTO>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image != null ? src.Image.URL : null));

            // Map for ProductByBrandCategoryDTO (this is typically built manually in controllers/managers)
            CreateMap<Product, ProductByBrandCategoryDTO>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.Products, opt => opt.Ignore()); // This will be populated manually
        }
    }
}