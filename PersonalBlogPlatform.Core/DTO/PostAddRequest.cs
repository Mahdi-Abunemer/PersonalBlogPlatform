﻿using PersonalBlogPlatform.Core.Domain.Entities;
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
        public required DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool? IsPublished { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
