using PersonalBlogPlatform.Core.Domain.Entities;

namespace PersonalBlogPlatform.Core.DTO
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }

        public required string CategoryName { get; set; }

        public required string Slug { get; set; }

        public List<Guid> PostIds { get; set; } = new();

        public Guid AuthorId { get; set; }
    }
}
