using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.DTO
{
    public class CategoryUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Category name can't be empty.")]
        public string CategoryName { get; set; } = default!;

        [Required, MaxLength(100)]
        public string Slug { get; set; } = default!;

        [Required]
        public Guid AuthorId { get; set; }
    }
}