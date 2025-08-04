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
    public class CommentAddProfileRequest : Profile
    {
        public CommentAddProfileRequest() 
        {
            CreateMap<CommentAddRequest, Comment>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Post, o => o.Ignore())   
            .ForMember(d => d.Author, o => o.Ignore()); 
        }
    }
}
