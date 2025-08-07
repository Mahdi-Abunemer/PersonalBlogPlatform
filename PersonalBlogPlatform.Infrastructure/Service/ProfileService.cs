using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.DTO;
using PersonalBlogPlatform.Core.Enums;
using PersonalBlogPlatform.Core.Exceptions;
using PersonalBlogPlatform.Core.Helper;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Core.Token;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Service
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;

        public ProfileService (UserManager<ApplicationUser> userManager , IHttpContextAccessor httpContext , IMapper mapper , SignInManager<ApplicationUser> signInManager , RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _contextAccessor = httpContext;
            _mapper = mapper;
            _signInManager = signInManager; 
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = _contextAccessor
                .HttpContext
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Access denied: user is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            return user;
        }

        public async Task<ApplicationUser> GetUserByRefreshToken(string refreshToken)
        {
            refreshToken = refreshToken?.Trim();

            var user = await _userManager.Users.FirstOrDefaultAsync(r => r.RefreshToken == refreshToken);
            if(user == null)
                throw new NotFoundException("User not found");

            return user;
        }

        public bool ValidateRefreshToken(ApplicationUser user, string refreshToken)
        {
          return user.RefreshToken == refreshToken && user.RefreshExpirationTime > DateTime.UtcNow;
        }

        public async Task<ApplicationUser> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                throw new AuthenticationException("Invalid email or password.");

            var signInResult = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
            if (signInResult.IsLockedOut)
                throw new AuthenticationException("Account locked. Please try again later.");

            if (!signInResult.Succeeded)
                throw new AuthenticationException("Invalid email or password.");

            return user;
        }

        public async Task Logout()
        {
            var user = await GetCurrentUserAsync();

            user.RefreshToken = null;
            user.RefreshExpirationTime = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            await _signInManager.SignOutAsync();
        }

        public async Task<ApplicationUser> Register(RegisterDto registerDto)
        {
           ValidationHelper.ModelValidation(registerDto);

           var IsUserEmailExists =  await _userManager.FindByEmailAsync(registerDto.Email);

            if (IsUserEmailExists != null)
                throw new InvalidOperationException("Email is already exists.");

            var applicationUser = _mapper.Map<ApplicationUser>(registerDto);

            applicationUser.Id = Guid.NewGuid();
            applicationUser.UserName = registerDto.Email;

            var result = await _userManager.CreateAsync(applicationUser ,registerDto.Password);

            if (!result.Succeeded)
                throw new ApplicationException(string.Join(",", result.Errors.Select(e => e.Description)));
            try
            {
                if (registerDto.UserType == UserTypeOptions.Admin)
                {
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        {
                            Name = UserTypeOptions.Admin.ToString()
                        };
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    var roleResult =
                        await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.Admin.ToString());

                    if (!roleResult.Succeeded)
                        throw new ApplicationException(
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));

                }
                else
                {
                    if (await _roleManager.FindByNameAsync(UserTypeOptions.User.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        {
                            Name = UserTypeOptions.User.ToString()
                        };
                        await _roleManager.CreateAsync(applicationRole);
                    }
                    var roleResult =
                        await _userManager.AddToRoleAsync(applicationUser, UserTypeOptions.User.ToString());

                    if (!roleResult.Succeeded)
                        throw new ApplicationException(
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            catch
            {
                // Clean up the half-created user so email-exists check stays accurate
                await _userManager.DeleteAsync(applicationUser);
                throw;
            }
     
            return applicationUser;
        }

        public async Task StoreRefreshToken(ApplicationUser user, TokenResponse token)
        {
           user.RefreshToken = token.RefreshToken;
           user.RefreshExpirationTime = token.RefreshTokenExpirationTime;
           
           await _userManager.UpdateAsync(user);
        }

        public async Task<UserDto> GetUserProfileAsync()
        {
            var user = await GetCurrentUserAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task UpdateUserProfileAsync(UserDto userDto)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto));

            ValidationHelper.ModelValidation(userDto);

            var user = await GetCurrentUserAsync();

            _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApplicationException(string.Join(",", result.Errors.Select(e => e.Description)));
        }
    }
}