using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.Domain.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        public required string CategoryName { get; set; }

        [Required, MaxLength(100)]
        public string Slug { get; set; } = string.Empty; // URL‑friendly

        public virtual ICollection<Post>? Posts { get; set; }

        public required Guid CreatedById { get; set; }
        public required ApplicationUser CreatedBy { get; set; }
    }
}
