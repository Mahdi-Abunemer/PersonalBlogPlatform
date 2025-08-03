using PersonalBlogPlatform.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonalBlogPlatform.Core.ServiceContracts
{
    public interface ICommentService
    {
        Task<CommentResponse> AddComment(CommentAddRequest request);

        Task<CommentResponse> GetCommentByCommentId(Guid commentId);

        Task<List<CommentResponse>> GetAllCommentsByPostId(Guid postId);

        Task DeleteComment(Guid commentId);

        Task<CommentResponse> UpdateComment(CommentUpdateRequest request);
    }
}