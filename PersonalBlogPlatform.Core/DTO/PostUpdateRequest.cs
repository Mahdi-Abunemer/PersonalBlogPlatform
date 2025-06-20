using PersonalBlogPlatform.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.DTO
{
    public  class PostUpdateRequest
    {
        [Required(ErrorMessage ="Post Id can't be empty.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Post must have a title.")]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Post content can't be empty.")]
        public string? Content { get; set; }

        public string? PostDetails { get; set; }

        public DateTime CreatedAt { get; set; }

        [DataType(DataType.DateTime)]
        public required DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool? IsPublished { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
