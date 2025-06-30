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

        public async Task<bool> DeletePostByPostId(Guid postId)
        {
            Post? post = await _db.Posts.FindAsync(postId);
            if (post == null)
                return false;

            _db.Posts.Remove(post);
            int rowsDeleted = await _db.SaveChangesAsync();
            return rowsDeleted > 0 ;
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

        public async Task<Post?> UpdatePost(Post post)
        {
            Post? matchPost = await _db.Posts.FindAsync(post.Id);
            if (matchPost != null)
            {
                matchPost.UpdatedAt = DateTime.UtcNow;
                matchPost.Title = post.Title;
                matchPost.PostDetails = post.PostDetails;
                matchPost.Content = post.Content;
                matchPost.IsPublished = post.IsPublished;

                await _db.SaveChangesAsync();
            }
            return matchPost;
        }
    }
}
