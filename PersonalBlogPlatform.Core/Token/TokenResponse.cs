using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Token
{
    public class TokenResponse
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenExpirationTime { get; set; }
    }
}
