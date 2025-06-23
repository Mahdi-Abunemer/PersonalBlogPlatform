using PersonalBlogPlatform.Core.Domain.IdentityEntities;
using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.Domain.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? PostDetails { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        public bool? IsPublished { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public Guid CreatedById { get; set; }

        public ApplicationUser? CreatedBy { get; set; } = default!;
    }
}
