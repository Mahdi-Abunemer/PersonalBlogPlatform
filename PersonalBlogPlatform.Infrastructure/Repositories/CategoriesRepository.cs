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

        public async Task<Category?> GetCategoryByCategoryId(Guid Id)
        {
           return await _db.Categories.Include(c => c.Posts).FirstOrDefaultAsync(c => c.Id == Id);
        }
    }
}
