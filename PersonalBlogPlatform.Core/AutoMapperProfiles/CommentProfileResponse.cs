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
    public class CommentProfileResponse : Profile
    {
        public CommentProfileResponse() 
        {
            CreateMap<Comment, CommentResponse>();
        }
    }
}
