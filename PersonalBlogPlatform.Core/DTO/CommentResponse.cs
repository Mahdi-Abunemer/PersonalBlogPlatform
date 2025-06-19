using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.DTO
{
    public class CommentResponse
    {

        public Guid Id { get; set; }
        public required string contentText { get; set; }

        public Guid? PostId { get; set; }
    }
}
