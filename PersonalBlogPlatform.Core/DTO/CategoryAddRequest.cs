using System.ComponentModel.DataAnnotations;

namespace PersonalBlogPlatform.Core.DTO
{
    public class CategoryAddRequest
    {
        [Required(ErrorMessage ="Category name can't be empty.")]
        public string? CategoryName { get; set; }

        [Required, MaxLength(100)]
        public string? Slug { get; set; }
    }
}
