using AutoMapper;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>();

            CreateMap<UserDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
