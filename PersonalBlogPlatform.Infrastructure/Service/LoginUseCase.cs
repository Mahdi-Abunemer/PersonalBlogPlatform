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
    public class LoginUseCase
    {
        private readonly IProfileService _profileService;
        private readonly ITokenService _tokenService;

        public LoginUseCase(IProfileService profileService , ITokenService tokenService)
        {
            _profileService = profileService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> LoginUserAsync (LoginDto loginDto)
        {
            var user = await _profileService.Login(loginDto);

            var token = await _tokenService.CreateJwtToken(user);

            await _profileService.StoreRefreshToken(user, token);

            return token;
        }
    }
}
