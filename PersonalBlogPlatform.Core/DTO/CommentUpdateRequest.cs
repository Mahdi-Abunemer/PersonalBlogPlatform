using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.DTO
{
    public class CommentUpdateRequest
    {
        [Required]
        public Guid Id { get; set; }

        [Required, StringLength(1000)]
        public string ContentText { get; set; } = string.Empty;

        [Required]
        public Guid PostId { get; set; }

        [Required]
        public Guid AuthorId { get; set; }
    }
}
