using AutoMapper;
using CMS.Server.Controllers.Images.DTO;
using CMS.Server.Models;

namespace CMS.Server.Controllers.Images
{
    public class ImageAutoMapper : Profile
    {
        public ImageAutoMapper()
        {
            // Update mapping for ImageGetDTO to work with direct ClothId and ColorId
            CreateMap<Image, ImageGetDTO>()
                .ForMember(dest => dest.ClothId, opt => opt.MapFrom(src => src.ClothId))
                .ForMember(dest => dest.ColorId, opt => opt.MapFrom(src => src.ColorId))
                .ForMember(dest => dest.ColorName, opt => opt.MapFrom(src => src.ClothColor.Color.ColorName))
                .ForMember(dest => dest.ClothName, opt => opt.MapFrom(src => src.ClothColor.Cloth.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ClothColor.Cloth.Price))
                .ForMember(dest => dest.AvailableStock, opt => opt.MapFrom(src => src.ClothColor.AvailableStock));

            // Update mapping for DTOs to entity
            CreateMap<ImageCreateDTO, Image>()
                .ForMember(dest => dest.ClothId, opt => opt.MapFrom(src => src.ClothId))
                .ForMember(dest => dest.ColorId, opt => opt.MapFrom(src => src.ColorId));

            CreateMap<ImageUpdateDTO, Image>()
                .ForMember(dest => dest.ClothId, opt => opt.MapFrom(src => src.ClothId))
                .ForMember(dest => dest.ColorId, opt => opt.MapFrom(src => src.ColorId));
        }
    }
}