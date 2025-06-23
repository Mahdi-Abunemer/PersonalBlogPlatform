using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public  string CategoryName { get; set; }

        [Required, MaxLength(100)]
        public string Slug { get; set; } = string.Empty; // URL‑friendly

        public virtual ICollection<Post>? Posts { get; set; }

        [Required]
        public  Guid CreatedById { get; set; }
        
        public  ApplicationUser? CreatedBy { get; set; }
    }
}
