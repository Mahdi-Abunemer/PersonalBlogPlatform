using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.DTO
{
    public class CommentAddRequest
    {
        [Required(ErrorMessage ="Comment can't be empty.")]
        [StringLength(1000)]
        public  string? ContentText { get; set; }

        public Guid? PostId { get; set; }
    }
}
