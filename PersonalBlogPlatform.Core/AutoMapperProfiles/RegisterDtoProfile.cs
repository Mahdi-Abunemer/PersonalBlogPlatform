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
    public class RegisterDtoProfile : Profile
    {
        public RegisterDtoProfile() 
        {
            CreateMap<RegisterDto, ApplicationUser>();
        }
    }
}
