
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

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Category>? Categories { get; set; }
    }

    public static class PostExtensions
    {
        public static PostResponse ToPostResponse(this Post post)
        {
            return new PostResponse
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                PostDetails = post.PostDetails,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                IsPublished = post.IsPublished,
                Comments = post.Comments,
                Categories = post.Categories
            };
        }
    }
}
