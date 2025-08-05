using AutoMapper;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.AutoMapperProfiles
{
    public class CategoryProfileResponse : Profile
    {
        public CategoryProfileResponse() 
        {
            CreateMap<Category, CategoryResponse>().
                ForMember(dest => dest.PostIds,
                    opt => opt.MapFrom(src =>
                        (src.Posts ?? Enumerable.Empty<Post>())
                            .Select(c => c.Id)
                            .ToList()));
        }
    }
}
