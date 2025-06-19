using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalBlogPlatform.Core.Domain.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(1000)]
        public required string contentText { get; set; }

        public Guid? PostId { get; set; }
    }
}
