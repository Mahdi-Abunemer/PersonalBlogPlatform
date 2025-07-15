using PersonalBlogPlatform.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.DTO
{
    public class PostResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }

        public required string Content { get; set; }

        public string? PostDetails { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsPublished { get; set; }

        public Guid AuthorId { get; set; }

        public List<Guid>? CommentIds { get; set; }

        public List<Guid>? CategoryIds { get; set; }
    }
}
