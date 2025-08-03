using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.DTO
{
    public class CommentResponse
    {
        public Guid Id { get; set; }

        public string contentText { get; set; }

        public Guid PostId { get; set; }

        public Guid AuthorId { get; set; }
    }
}
