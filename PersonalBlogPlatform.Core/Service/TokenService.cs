using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using PersonalBlogPlatform.Core.ServiceContracts;
using PersonalBlogPlatform.Core.Token;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        //TO DO: Move all implementations of interfaces into Infrastructure layer 
        private readonly UserManager<ApplicationUser> _userManager;
        public TokenService (IConfiguration configuration , UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<TokenResponse> CreateJwtToken(ApplicationUser applicationUser)
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes
                (Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            var roles = await _userManager.GetRolesAsync(applicationUser);
            var role = roles.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(role))
                throw new InvalidOperationException("The user does not have a valid role assigned.");

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub , applicationUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat ,DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, applicationUser.DisplayName ?? string.Empty),
                new Claim(ClaimTypes.Role , role.ToString())
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            SigningCredentials signingCredentials = new SigningCredentials
                (securityKey , SecurityAlgorithms.HmacSha256);

            JwtSecurityToken jwtGenerator = new JwtSecurityToken
                (
                  issuer: _configuration["Jwt:Issuer"],
                  audience: _configuration["Jwt:Audience"],
                  claims: claims,
                  expires : expirationTime,
                  signingCredentials : signingCredentials
                );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(jwtGenerator);

            return new TokenResponse
            {
                Token = token, 
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationTime = DateTime.Now.AddMinutes
                      (Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"]))
            };
        }

        private string GenerateRefreshToken()
        {
            byte[] bytes = new byte[64];

           var randomNumberGenerator = RandomNumberGenerator.Create();
           randomNumberGenerator.GetBytes(bytes);

           return Convert.ToBase64String(bytes);
        }
    }
}
