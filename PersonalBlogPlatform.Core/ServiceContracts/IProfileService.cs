using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.ServiceContracts
{
    public interface IProfileService
    {
       public Task<ApplicationUser> GetCurrentUserAsync();

       public Task<ApplicationUser> Register(RegisterDto registerDto);

       public Task StoreRefreshToken(ApplicationUser user , TokenResponse token);

       public Task<ApplicationUser> Login(LoginDto loginDto);

       public Task Logout();

       public Task<ApplicationUser> GetUserByRefreshToken(string refreshToken);

       public bool ValidateRefreshToken(ApplicationUser user, string refreshToken);
    }
}