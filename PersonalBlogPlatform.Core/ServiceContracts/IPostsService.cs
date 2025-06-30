using PersonalBlogPlatform.Core.DTO;

namespace PersonalBlogPlatform.Core.ServiceContracts
{
    public interface IPostsService
    {
        Task<PostResponse> AddPost(PostAddRequest postAddRequest);

        Task<PostResponse?> UpdatePost(PostUpdateRequest postUpdateRequest);

        Task<bool> DeletePostByPostId(Guid postId);

        Task<List<PostResponse>> GetAllPosts();

        Task<PostResponse?> GetPostById(Guid postId);

        Task<List<PostResponse>> GetLatestPosts(int count = 5);

        Task<List<PostResponse>> GetFilteredPosts(Guid categoryId);
    }
}
