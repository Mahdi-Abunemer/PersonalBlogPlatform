using PersonalBlogPlatform.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.DTO
{
    public  class PostAddRequest
    {
        [Required(ErrorMessage ="Post must have a title.")]
        [StringLength(100)]
        public  string? Title { get; set; }

        [Required(ErrorMessage= "Post content can't be empty.")]
        public  string? Content { get; set; }

        public string? PostDetails { get; set; }

        [DataType(DataType.DateTime)]
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsPublished { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Category>? Categories { get; set; }

        public Post ToPost()
        {
            return new Post
            {
                Title = Title,
                Content = Content,
                PostDetails = PostDetails,
                CreatedAt = CreatedAt,
                IsPublished = IsPublished,
                Comments = Comments,
                Categories = Categories
            }; 
        }
    }
}
