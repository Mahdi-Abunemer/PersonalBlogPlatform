using PersonalBlogPlatform.Core.Domain.Entities;

namespace PersonalBlogPlatform.Core.Domain.RepositoryContracts
{
    public interface ICategoriesRepository
    {
        Task<Category?> GetCategoryByCategoryId(Guid Id);
    }
}
