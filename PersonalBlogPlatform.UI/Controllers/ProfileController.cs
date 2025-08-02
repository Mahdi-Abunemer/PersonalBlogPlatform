using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Service;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Core.Token;

namespace PersonalBlogPlatform.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProfileController : ControllerBase
    {
        private readonly RegisterUseCase _registerUseCase;
        private readonly LoginUseCase _loginUseCase;
        private readonly IProfileService _profileService;
        private readonly RefreshTokenUseCase _refreshTokenUseCase;

        public ProfileController (RegisterUseCase registerUseCase, LoginUseCase loginUseCase, IProfileService profileService, RefreshTokenUseCase refreshTokenUseCase)
        {
            _registerUseCase = registerUseCase;
            _loginUseCase = loginUseCase;
            _profileService = profileService;
            _refreshTokenUseCase = refreshTokenUseCase;
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var token = await _registerUseCase.RegisterUserAsync(registerDto);

            return Ok(token);
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _loginUseCase.LoginUserAsync(loginDto);

            return Ok(token);
        }

        [HttpPost]
        [Route("[Action]")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _profileService.Logout();

            return NoContent();
        }

        [HttpPost]
        [Route("[Action]")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var newToken = await _refreshTokenUseCase.RenewAccessTokenAsync(request.RefreshToken);

            return Ok(newToken);
        }

        [HttpGet]
        [Route("[Action]")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile()
        {
            var user = await _profileService.GetUserProfileAsync();
            return Ok(user);
        }

        [HttpPut]
        [Route("[Action]")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfile(UserDto userDto)
        {
            await _profileService.UpdateUserProfileAsync(userDto);
            return NoContent();
        }
    }
}
