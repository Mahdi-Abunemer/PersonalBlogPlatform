using AutoMapper;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.DTO;

namespace PersonalBlogPlatform.Core.AutoMapperProfiles
{
    public class PostResponseProfile : Profile
    {
        public PostResponseProfile() 
        {
            CreateMap<Post, PostResponse>()
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments)) 
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));
        }
    }
}
