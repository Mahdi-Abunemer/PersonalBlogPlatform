using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Infrastructure.DbContext;

namespace PersonalBlogPlatform.Infrastructure.Repositories
{
    public class PostsRepository : IPostsRepository
    {
        private readonly ApplicationDbContext _db; 

        public PostsRepository ( ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Post> AddPost(Post post)
        {
           _db.Posts.Add(post);
           await _db.SaveChangesAsync();
           return post;
        }

        public async Task DeletePost(Post post)
        {
            _db.Posts.Remove(post);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Post>> GetAllPosts()
        {
           return await _db.Posts.Include(p => p.Categories).ToListAsync();
        }

        public async Task<List<Post>> GetFilteredPosts(Guid categoryId)
        {
           return await _db.Posts
           .Include(p => p.Categories)
           .Where(p => p.Categories.Any(c => c.Id == categoryId))
           .ToListAsync();
        }

        public async Task<List<Post>> GetLatestPosts(int count = 5)
        {
            return await _db.Posts.Include(p => p.Categories)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public  async Task<Post?> GetPostByPostId(Guid postId)
        {
            return await _db.Posts.Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task UpdatePost(Post post)
        {
            _db.Posts.Update(post);
            await _db.SaveChangesAsync();
        }
    }
}
