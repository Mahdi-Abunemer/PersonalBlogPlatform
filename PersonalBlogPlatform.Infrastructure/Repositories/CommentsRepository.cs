using Microsoft.EntityFrameworkCore;
using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.Domain.RepositoryContracts;
using PersonalBlogPlatform.Infrastructure.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Infrastructure.Repositories
{
    public class CommentsRepository : ICommentsRepository
    {
        private readonly ApplicationDbContext _db;

        public CommentsRepository(ApplicationDbContext db) 
        {
            _db = db;
        }

        public async Task<Comment> AddComment(Comment comment)
        {
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();
            return comment;
        }

        public async Task DeleteComment(Comment comment)
        {
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Comment>> GetAllCommentsByPostId(Guid postId)
        {
            return await _db.Comments
                .Where(c => c.PostId == postId)
                .Include(c => c.Author)
                .Include(c => c.Post)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentByCommentId(Guid commentId)
        {
            return await _db.Comments.
                FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task UpdateComment(Comment comment)
        {
            _db.Comments.Update(comment);
            await _db.SaveChangesAsync();
        }
    }
}
