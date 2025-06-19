using System;
using System.ComponentModel.DataAnnotations;


namespace PersonalBlogPlatform.Core.Domain.Entities
{
    public class Post
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(100)]
        public required string Title { get; set; }

        public required string Content { get; set; }

        public string? PostDetails { get; set; }

        [DataType(DataType.DateTime)]
        public required DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        public bool? IsPublished { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
