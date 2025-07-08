using AutoMapper;
using CMS.Server.Controllers.Images.DTO;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Images
{
    public class ImageAutoMapper : Profile
    {
        public ImageAutoMapper()
        {
            // Update mapping for ImageGetDTO to work with direct BrandId and CategoryId
            CreateMap<Image, ImageGetDTO>()
                .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.BrandCategory.Category.CategoryName))
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.BrandCategory.Brand.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BrandCategory.Brand.Price))
                .ForMember(dest => dest.AvailableStock, opt => opt.MapFrom(src => src.BrandCategory.AvailableStock));

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