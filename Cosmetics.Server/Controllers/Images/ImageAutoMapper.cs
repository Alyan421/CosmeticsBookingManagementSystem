using AutoMapper;
using Cosmetics.Server.Controllers.Images.DTO;
using Cosmetics.Server.Models;

namespace Cosmetics.Server.Controllers.Images
{
    public class ImageAutoMapper : Profile
    {
        public ImageAutoMapper()
        {
            // Map Image entity to ImageGetDTO (updated for new schema)
            CreateMap<Image, ImageGetDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.AvailableProduct, opt => opt.MapFrom(src => src.Product.AvailableProduct))
                // Backward compatibility mappings
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.Product.BrandId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Product.CategoryId))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Product.Brand.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.CategoryName));

            // Map ImageCreateDTO to Image entity (updated for new schema)
            CreateMap<ImageCreateDTO, Image>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id is auto-generated
                .ForMember(dest => dest.Product, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.URL, opt => opt.Ignore()); // URL is set by the image storage service

            // Map ImageUpdateDTO to Image entity (updated for new schema)
            CreateMap<ImageUpdateDTO, Image>()
                .ForMember(dest => dest.Product, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.URL, opt => opt.Ignore()); // URL is handled separately

            // Backward compatibility mappings for legacy DTOs
            CreateMap<ImageCreateByBrandCategoryDTO, Image>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id is auto-generated
                .ForMember(dest => dest.ProductId, opt => opt.Ignore()) // Will be set based on brand/category lookup
                .ForMember(dest => dest.Product, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.URL, opt => opt.Ignore()); // URL is set by the image storage service

            CreateMap<ImageUpdateByBrandCategoryDTO, Image>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore()) // Will be set based on brand/category lookup
                .ForMember(dest => dest.Product, opt => opt.Ignore()) // Navigation property
                .ForMember(dest => dest.URL, opt => opt.Ignore()); // URL is handled separately
        }
    }
}