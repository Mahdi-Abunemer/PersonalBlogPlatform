using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Core.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Service
{
    public class RefreshTokenUseCase
    {
        private readonly IProfileService _profileService;
        private readonly ITokenService _tokenService;

        public RefreshTokenUseCase(IProfileService profileService, ITokenService tokenService)
        {
            _profileService = profileService;
            _tokenService = tokenService;
        }

        public async Task<TokenResponse> RenewAccessTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentNullException(nameof(refreshToken));

            var user = await _profileService.GetUserByRefreshToken(refreshToken);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid refresh token!");

            var isValid = _profileService.ValidateRefreshToken(user, refreshToken);
            if (!isValid)
                throw new UnauthorizedAccessException("Expired or invalid refresh token!");

            var newToken = await _tokenService.CreateJwtToken(user);
            await _profileService.StoreRefreshToken(user, newToken);

            return newToken;
        }
    }
}
