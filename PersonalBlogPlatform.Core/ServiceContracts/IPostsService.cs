using PersonalBlogPlatform.Core.Domain.Entities;
using PersonalBlogPlatform.Core.DTO;

namespace PersonalBlogPlatform.Core.ServiceContracts
{
    public interface IPostsService
    {
        Task<PostResponse> AddPost(PostAddRequest postAddRequest);

        Task<PostResponse> UpdatePost(PostUpdateRequest postUpdateRequest);

        Task DeletePost(Guid postId);

        Task<List<PostResponse>> GetAllPosts();

        Task<PostResponse> GetPostById(Guid postId);

        Task<List<PostResponse>> GetLatestPosts(int count);

        Task<List<PostResponse>> GetFilteredPosts(Guid categoryId);
    }
}
