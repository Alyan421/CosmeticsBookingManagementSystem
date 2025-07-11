using AutoMapper;
using Cosmetics.Server.Controllers.Images.DTO;
using Cosmetics.Server.Models;

namespace Cosmetics.Server.Controllers.Images
{
    public class ImageAutoMapper : Profile
    {
        public ImageAutoMapper()
        {
            // Update mapping for ImageGetDTO to work with direct BrandId and CategoryId
            CreateMap<Image, ImageGetDTO>()
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category.CategoryName))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Product.Brand.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price)) // Updated: Get Price from Product
                .ForMember(dest => dest.AvailableProduct, opt => opt.MapFrom(src => src.Product.AvailableProduct));

            // Update mapping for DTOs to entity
            CreateMap<ImageCreateDTO, Image>()
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));

            CreateMap<ImageUpdateDTO, Image>()
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
        }
    }
}