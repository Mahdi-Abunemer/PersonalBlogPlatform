using PersonalBlogPlatform.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing Comment entity
    /// </summary>
    public interface ICommentsRepository
    {
        Task <Comment> AddComment(Comment comment);

        Task<Comment?> GetCommentByCommentId(Guid commentId);
        
        Task<List<Comment>> GetAllCommentsByPostId(Guid postId);

        Task DeleteComment(Comment comment);

        Task UpdateComment(Comment comment);
    }
}
