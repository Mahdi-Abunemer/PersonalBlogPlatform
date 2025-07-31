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

        public ProfileController (RegisterUseCase registerUseCase)
        {
            _registerUseCase = registerUseCase; 
        }

        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var token = await _registerUseCase.RegisterUserAsync(registerDto);

            return Ok(token);
        }
    }
}
