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
    public class UserDtoProfile : Profile
    {
        public UserDtoProfile() 
        {
            CreateMap<UserDto, ApplicationUser>();
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
