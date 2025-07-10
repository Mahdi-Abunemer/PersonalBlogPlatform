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
                .ForMember(dest => dest.CategoryIds,
                    opt => opt.MapFrom(src =>
                        (src.Categories ?? Enumerable.Empty<Category>())
                            .Select(c => c.Id)
                            .ToList()))
                .ForMember(dest => dest.CommentIds,
                    opt => opt.MapFrom(src =>
                        (src.Comments ?? Enumerable.Empty<Comment>())
                            .Select(c => c.Id)
                            .ToList()));
        }
    }
}
