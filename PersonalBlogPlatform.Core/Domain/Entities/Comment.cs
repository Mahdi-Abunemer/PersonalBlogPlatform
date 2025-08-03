using PersonalBlogPlatform.Core.Domain.IdentityEntities;
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
        public required string ContentText { get; set; }
        
        public required Guid PostId { get; set; }
        public virtual required Post Post { get; set; }

        public required Guid AuthorId { get; set; }           
        public virtual required ApplicationUser Author { get; set; } 
    }
}
