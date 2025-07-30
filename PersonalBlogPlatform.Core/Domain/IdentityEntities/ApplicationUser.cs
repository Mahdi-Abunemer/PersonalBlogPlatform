using Microsoft.AspNetCore.Identity;
using PersonalBlogPlatform.Core.Domain.Entities;


namespace PersonalBlogPlatform.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }

        public virtual ICollection<Category>? Categories { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }

        public string? RefreshToken { get; set; } = null; 
        public DateTime RefreshExpirationTime { get; set; }
    }
}
