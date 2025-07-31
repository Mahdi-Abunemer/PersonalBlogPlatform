using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Core.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Service
{
    public class RegisterUseCase
    {
        private readonly IProfileService _profileService;
        private readonly ITokenService _tokenService;

        public RegisterUseCase(IProfileService profileService, ITokenService tokenService)
        {
            _profileService = profileService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> RegisterUserAsync(RegisterDto registerDto)
        {
           var user = await _profileService.Register(registerDto);

           var token = await _tokenService.CreateJwtToken(user);

           await _profileService.StoreRefreshToken(user, token);
            
           return token;
        }
    }
}
