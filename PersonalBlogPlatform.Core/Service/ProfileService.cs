using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.Exceptions;
using PersonalBlogPlatform.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Service
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProfileService (UserManager<ApplicationUser> userManager , IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _contextAccessor = httpContext;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = _contextAccessor
                .HttpContext
                .User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Access denied: user is not authenticated.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            return user;
        }
    }
}
