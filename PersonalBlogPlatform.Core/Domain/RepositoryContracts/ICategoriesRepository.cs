using PersonalBlogPlatform.Core.Domain.Entities;

namespace PersonalBlogPlatform.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Category entity
    /// </summary>
    public interface ICategoriesRepository
    {
        Task<Category?> GetCategoryByCategoryId(Guid Id);

        Task<Category> AddCategory(Category category);

        Task UpdateCategory(Category category);

        Task DeleteCategory(Category category);
    }
}
