using AutoMapper;
using ImageGalleries.WebApi.DTOs;
using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Picture, PictureDto>();
            CreateMap<Picture, PictureDto>().ReverseMap();

            CreateMap<User, UserDto>();
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Comment, CommentDto>();
            CreateMap<Comment, CommentDto>().ReverseMap();

            CreateMap<Tag, TagDto>();
            CreateMap<Tag, TagDto>().ReverseMap();

            CreateMap<Gallery, GalleryDto>();
            CreateMap<Gallery, GalleryDto>().ReverseMap();

            CreateMap<Score, ScoreDto>();
            CreateMap<Score, ScoreDto>().ReverseMap();
        }
    }
}
