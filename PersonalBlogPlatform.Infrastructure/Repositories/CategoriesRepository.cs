using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Infrastructure.DbContext;

namespace PersonalBlogPlatform.Infrastructure.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly ApplicationDbContext _db;
        
        public CategoriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Category> AddCategory(Category category)
        {
            await _db.Categories.AddAsync(category);
            await _db.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategory(Category category)
        {
            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
        }

        public async Task<Category?> GetCategoryByCategoryId(Guid Id)
        {
           return await _db.Categories.Include(c => c.Posts).FirstOrDefaultAsync(c => c.Id == Id);
        }

        public async Task UpdateCategory(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
        }
    }
}
