using AutoMapper;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Domain.Entities;

namespace PersonalBlogPlatform.Core.AutoMapperProfiles
{
    public  class PostAddRequestProfile : Profile
    {
        public PostAddRequestProfile() 
        {
            CreateMap<PostAddRequest, Post>();
        }
    }
}
